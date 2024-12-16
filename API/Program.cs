using API;
using API.Data;
using API.Middleware;
using API.Helpers;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.RateLimit;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<DataContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("ITDConn")));

builder.Services.AddScoped<IGreetingRepository, GreetingRepository>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// Configure Polly Rate Limiting Policy
var rateLimiterConfig = builder.Configuration.GetSection("RateLimiter").Get<RateLimiterConfig>();
var rateLimitPolicy = Policy.RateLimitAsync(
    rateLimiterConfig.Requests, // Max requests from configuration
    TimeSpan.FromSeconds(rateLimiterConfig.Time) // Time window from configuration
);

// Register the policy
builder.Services.AddSingleton(rateLimitPolicy);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseMiddleware<RateLimitingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();

app.Run();

