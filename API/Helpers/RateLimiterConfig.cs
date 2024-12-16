using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class RateLimiterConfig
    {
        public int Requests { get; set; }
        public int Time { get; set; }
    }
}