using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using Polly.RateLimit;

namespace API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AsyncRateLimitPolicy _rateLimitPolicy;

    public RateLimitingMiddleware(RequestDelegate next, AsyncRateLimitPolicy rateLimitPolicy)
    {
        _next = next;
        _rateLimitPolicy = rateLimitPolicy;
    }

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
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
        }
    }
}


