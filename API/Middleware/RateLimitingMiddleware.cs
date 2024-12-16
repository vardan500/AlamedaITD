using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using Polly.RateLimit;

namespace API.Middleware;

public class RateLimitingMiddleware(RequestDelegate _next, AsyncRateLimitPolicy _rateLimitPolicy, ILogger<RateLimitingMiddleware> _logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Execute the rate limiter policy
            await _rateLimitPolicy.ExecuteAsync(async () =>
            {
                await _next(context); // Continue with the next middleware if allowed
            });
        }
        catch (RateLimitRejectedException)
        {
            // Respond with 429 Too Many Requests
            _logger.LogWarning("Rate limit exceeded!");
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
        }
    }
}


