using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ShortUrl
{
    [BsonId]
    public string id { get; set;} = Guid.NewGuid().ToString();
    public string LongUrl { get; set;}
    public string ShortCode { get; set;}
    public int ClickCount { get; set;}
    public DateTime CreatedAt { get; set;}
}