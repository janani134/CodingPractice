public interface IShortUrlService
{
    Task<string> ShortenUrlAsync(string longUrl);
    Task<string> GetLongUrlAsync(string shortUrl);
}