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

namespace API.Controllers
{
    [Route("[controller]")]
    public class GreetingController(DataContext context) : Controller
    {
        // private readonly ILogger<GreetingsController> _logger;

        // public GreetingsController(ILogger<GreetingsController> logger)
        // {
        //     _logger = logger;
        // }

        [HttpGet]
        public async Task<ActionResult> GetGreeting(string Name)
        {

            if (String.IsNullOrEmpty(Name))
            {
                return Ok("Hello, World!");
            }

            var greeting = await context.Greetings.FirstOrDefaultAsync(x => x.Name == Name);

            if (greeting == null)
            {
                return Ok("Hello, World!");
            }

            return Ok(greeting.Greetings);
        }

        [HttpPost]
        public async Task<ActionResult<string>> Create(GreetingDto greetingDto)
        {
            if (greetingDto == null || String.IsNullOrEmpty(greetingDto.Name) || String.IsNullOrEmpty(greetingDto.Greetings))
            {
                return BadRequest("Missing request data");
            }

            var greetingRecord = await context.Greetings.FirstOrDefaultAsync(n => n.Name == greetingDto.Name);
            if (greetingRecord == null)
            {
                context.Greetings.Add(new Greeting { Name = greetingDto.Name, Greetings = greetingDto.Greetings });
            }
            else
            {
                greetingRecord.Greetings = greetingDto.Greetings;
                context.Entry(greetingRecord).State = EntityState.Modified;
            }

            await context.SaveChangesAsync();

            return NoContent("Created Successfully");
        }

    }
}