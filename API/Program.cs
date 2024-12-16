using API;
using API.Data;
using API.Middleware;
using API.Helpers;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.RateLimit;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("ITDConn")));
builder.Services.AddScoped<IGreetingRepository, GreetingRepository>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// Configure Polly Rate Limiting Policy
var rateLimitPolicy = Policy.RateLimitAsync(
    5,                              // Max 5 requests
    TimeSpan.FromSeconds(1)        // Per 1 seconds
);

// Register the policy
builder.Services.AddSingleton(rateLimitPolicy);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
