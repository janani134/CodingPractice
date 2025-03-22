public class ShortUrlService : IShortUrlService
{
    private readonly IShortUrlRepository _repo;
    private const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
    private static readonly Random RandomGenerator = new();

    public ShortUrlService(IShortUrlRepository repo)
    {
        _repo = repo;
    }

    public async Task<string> ShortenUrlAsync(string longUrl)
    {
        var existingUrl = await _repo.GetUrlLongUrlAsync(longUrl);
        if(existingUrl != null)
        {
            return existingUrl.ShortCode;
        }
        string sCode;
        do
        {
            sCode = GenerateShortCode();
        }while (await _repo.GetUrlByShortCodeAsync(sCode) != null);

        var uriModel = new ShortUrl
        {
            id = Guid.NewGuid().ToString(),
            LongUrl = longUrl,
            ShortCode = sCode,
            ClickCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.CreateAsync(uriModel);
        return sCode;
    }

    public async Task<string> GetLongUrlAsync(string sCode)
    {
        Console.WriteLine("Get the long url");
        var uri = await _repo.GetUrlByShortCodeAsync(sCode);

        if(uri != null)
        {
            Console.WriteLine("Fetched data");
            await _repo.IncrementClickCountAsync(sCode);
            return uri.LongUrl;
        }

        return null;
    }

    private string GenerateShortCode(int len = 7)
    {
        return new string(Enumerable.Repeat(characters, len).Select(s => s[RandomGenerator.Next(s.Length)]).ToArray());
    }
}
