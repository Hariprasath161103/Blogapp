using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Blogapp.Models
{
    public class BlogPost
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string? ImageUrl { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("Likes")]
        public int Likes { get; set; } = 0;

        [BsonElement("Comments")]
        public List<string> Comments { get; set; } = new();

        [BsonElement("AuthorEmail")]
        public string AuthorEmail { get; set; } = string.Empty;
    }
}
