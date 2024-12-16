using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GreetingClient
{
    class Program
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly Dictionary<string, string> GreetingCache = new Dictionary<string, string>();

        static async Task Main(string[] args)
        {
            string baseUrl = "http://localhost:5026"; // Replace with your API base URL.

            Console.WriteLine("Welcome to the Greeting App!");

            while (true)
            {
                Console.WriteLine("\nEnter your name (or type 'exit' to quit): ");
                string name = Console.ReadLine();

                if (string.Equals(name, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Name cannot be empty. Please try again.");
                    continue;
                }

                try
                {
                    string greeting = await GetGreetingAsync(baseUrl, name);
                    Console.WriteLine(greeting);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private static async Task<string> GetGreetingAsync(string baseUrl, string name)
        {
            if (GreetingCache.ContainsKey(name))
            {
                Console.WriteLine("(Cache hit)");
                return GreetingCache[name];
            }

            string requestUrl = $"{baseUrl}/greeting?Name={Uri.EscapeDataString(name)}";

            HttpResponseMessage response = await HttpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                string greeting = await response.Content.ReadAsStringAsync();
                GreetingCache[name] = greeting; // Store in cache.
                return greeting;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                throw new Exception("Rate limit exceeded. Please try again later.");
            }
            else
            {
                throw new Exception($"Error from server: {response.StatusCode}");
            }
        }
    }
}

