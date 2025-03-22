public interface IShortUrlRepository
{
    Task<ShortUrl> GetUrlByShortCodeAsync(string shortCode);
    Task<ShortUrl> GetUrlLongUrlAsync(string shortCode);
    Task CreateAsync(ShortUrl shortUrl);
    Task DeleteAsync(string shortCode);
    Task IncrementClickCountAsync(string shortCode);


}