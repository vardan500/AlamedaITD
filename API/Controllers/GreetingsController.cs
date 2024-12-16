using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using API.Dto;
using Polly;
using Polly.RateLimit;
using System.Collections.Generic;

namespace API.Controllers
{

    [Route("[controller]")]
    public class GreetingController(IGreetingRepository repository, ICacheService cacheService, ILogger<GreetingController> _logger) : Controller
    {

        // Get the greeting based on the name. If name is empty, use "default"
        [HttpGet]
        public async Task<ActionResult> GetGreeting(string Name)
        {

            if (String.IsNullOrEmpty(Name))
            {
                Name = "default"; // Default name if no input
            }

            string cacheKey = "greetingsKey"; // Cache key used for greeting data
            var cachedData = cacheService?.Get<List<Greeting>>(cacheKey) ?? new List<Greeting>(); // Get cached data or an empty list if not found
            var data = cachedData.FirstOrDefault(g => g.Name == Name); // Check if the greeting is already cached

            if (data == null)
            {
                // If data not found in cache, retrieve it from the database
                var dataFromDB = await repository.GetGreetingAsync(Name);

                if (dataFromDB == null)
                {
                    return Ok("Hello, World!"); // Return default greeting if no data in DB
                }
                else
                {
                    // Add the newly retrieved data to the cache
                    cachedData.Add(dataFromDB);

                    // Update the cache with the new list of greetings
                    cacheService?.Set(cacheKey, cachedData, TimeSpan.FromMinutes(5));

                    // Return the greeting retrieved from DB
                    return Ok(dataFromDB.Greetings);
                }
            }
            else
            {
                _logger.LogInformation("Cache hit!");
            }

            return Ok(data.Greetings);
        }

        // Create a new greeting
        [HttpPost]
        public async Task<ActionResult<string>> Create(GreetingDto greetingDto)
        {
            if (greetingDto == null || String.IsNullOrEmpty(greetingDto.Name) || String.IsNullOrEmpty(greetingDto.Greetings))
            {
                return BadRequest("Missing data");
            }

            await repository.SaveGreetingAsync(greetingDto);

            // Update cache
            string cacheKey = "greetingsKey";
            var cachedGreetings = cacheService.Get<List<Greeting>>(cacheKey) ?? new List<Greeting>();

            var existingGreeting = cachedGreetings.Find(g => g.Name == greetingDto.Name);
            if (existingGreeting != null)
            {
                // Update existing greeting in cache
                existingGreeting.Greetings = greetingDto.Greetings;
            }
            else
            {
                // Add new greeting to cache
                var newGreeting = await repository.GetGreetingAsync(greetingDto.Name);
                cachedGreetings.Add(newGreeting);
            }

            cacheService.Set(cacheKey, cachedGreetings, TimeSpan.FromMinutes(5));

            return NoContent();
        }

    }
}