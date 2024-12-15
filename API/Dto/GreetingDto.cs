using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dto
{
    public class GreetingDto
    {
        public required string Name { get; set; }

        public required string Greetings { get; set; }
    }
}