using MongoDB.Driver;
using Microsoft.Extensions.Options;

public class ShortUrlRepository : IShortUrlRepository
{
    private readonly IMongoCollection<ShortUrl> _coll ;

    public ShortUrlRepository( IOptions<ShortUrlDBSetting> shortUrlDBSetting, IMongoClient mongoClient)
    {
        var mongoDatabase = mongoClient.GetDatabase(shortUrlDBSetting.Value.databaseName);

        _coll = mongoDatabase.GetCollection<ShortUrl>(shortUrlDBSetting.Value.collectionName);
    }

    public async Task<ShortUrl> GetUrlByShortCodeAsync(string shortCode)
    {
        return await _coll.Find(u => u.ShortCode == shortCode).FirstOrDefaultAsync();
    }

    public async Task<ShortUrl> GetUrlLongUrlAsync(string longUrl)
    {
        return await _coll.Find(u => u.LongUrl == longUrl).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(ShortUrl shortUrl)
    {
        if(shortUrl.id == null)
        {
            shortUrl.id = Guid.NewGuid().ToString();
        }
        await _coll.InsertOneAsync(shortUrl);
    }

    public async Task DeleteAsync(string shortCode)
    {
        await _coll.DeleteOneAsync(u => u.ShortCode == shortCode);
    }

    public async Task IncrementClickCountAsync(string shortUrl)
    {
        var uri = await _coll.Find( u => u.ShortCode == shortUrl).FirstOrDefaultAsync();
        if(uri != null)
        {
            await _coll.UpdateOneAsync(
                Builders<ShortUrl>.Filter.Eq("id", uri.id),
                Builders<ShortUrl>.Update.Inc(u => u.ClickCount, 1)
            );
        }
    }


}