using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dto;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class GreetingRepository(DataContext context) : IGreetingRepository
    {

        public async Task<IEnumerable<Greeting>> GetGreetingsAllAsync()
        {
            return await context.Greetings.ToListAsync();
        }

        public async Task<Greeting> GetGreetingAsync(string Name)
        {
            return await context.Greetings.FirstOrDefaultAsync(x => x.Name == Name);
        }

        public async Task<bool> SaveGreetingAsync(GreetingDto greetingDto)
        {
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
            return true;
        }
    }

    public interface IGreetingRepository
    {

        public Task<IEnumerable<Greeting>> GetGreetingsAllAsync();

        public Task<Greeting> GetGreetingAsync(string Name);

        Task<bool> SaveGreetingAsync(GreetingDto greetingDto);

    }
}