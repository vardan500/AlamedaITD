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
    public class GreetingController(IGreetingRepository repository, ICacheService cacheService) : Controller
    {

        [HttpGet]
        public async Task<ActionResult> GetGreeting(string Name)
        {

            if (String.IsNullOrEmpty(Name))
            {
                Name = "default";
            }

            string cacheKey = "greetingsKey";
            var cachedData = cacheService?.Get<List<Greeting>>(cacheKey) ?? new List<Greeting>();
            var data = cachedData.FirstOrDefault(g => g.Name == Name);

            if (data == null)
            {
                // Retrieve the data from the database
                var dataFromDB = await repository.GetGreetingAsync(Name);

                if (dataFromDB == null)
                {
                    return Ok("Hello, World!");
                }
                else
                {
                    // Add the new data to the cached list
                    cachedData.Add(dataFromDB);

                    // Update the cache with the updated list
                    cacheService?.Set(cacheKey, cachedData, TimeSpan.FromMinutes(5));

                    return Ok(dataFromDB.Greetings);
                }
            }

            return Ok(data.Greetings);
        }

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