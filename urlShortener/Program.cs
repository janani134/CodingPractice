using MongoDB.Driver;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "URL Shortener API",
        Version = "v1",
        Description = "A simple URl shortening api"
    });
});

builder.Services.Configure<ShortUrlDBSetting>(builder.Configuration.GetSection("ShortUrlDBSettings"));

builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(sp.GetRequiredService<IOptions<ShortUrlDBSetting>>().Value.connectionString));
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<ShortUrlDBSetting>>().Value;
    return sp.GetRequiredService<IMongoClient>().GetDatabase(settings.databaseName);
});

builder.Services.AddSingleton<IShortUrlRepository, ShortUrlRepository>();
builder.Services.AddSingleton<IShortUrlService, ShortUrlService>();
var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "URL Shortner API"));
}

app.MapPost("/api/shorten", async (string longUrl, IShortUrlService shortUrlService) =>
{
    string shortCode = await shortUrlService.ShortenUrlAsync(longUrl);
    return Results.Ok(new { shortUrl = $"http://localhost:5166/{shortCode}"});
});

app.MapGet("/{shortCode}", async (string shortCode, IShortUrlService shortUrlService) => 
{
    var longUrl = await shortUrlService.GetLongUrlAsync(shortCode);
    if(!Uri.TryCreate(longUrl, UriKind.Absolute, out Uri ? validatedUri) ||
    (validatedUri.Scheme != Uri.UriSchemeHttp && validatedUri.Scheme != Uri.UriSchemeHttps))
    {
        Console.WriteLine("Inavlid Uri");   
    }
    return longUrl is not null ? Results.Redirect(longUrl) : Results.NotFound("Short url not found");
});

app.Run();
