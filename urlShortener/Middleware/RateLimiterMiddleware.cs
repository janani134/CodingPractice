using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, (DateTime, int)> _requestCounts = new();
    private static readonly TimeSpan TimeWindow = TimeSpan.FromSeconds(10);
    private const int requestLimit = 3;

    public RateLimiterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        //get user Ip address
        string userKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        DateTime now = DateTime.UtcNow;

        //check if userkey is in dictionary
        _requestCounts.AddOrUpdate(userKey,
        _ => (now, 1),
        (_, existing) =>
        {
            if(now - existing.Item1 > TimeWindow)
                return (now, 1);
            return (existing.Item1, existing.Item2 + 1);
        });
        
        //check if rate limit has exceeded
        if(_requestCounts[userKey].Item2 > requestLimit)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too Many Requests. Please try Again");
            return;
        }
        await _next(context);

    }
}