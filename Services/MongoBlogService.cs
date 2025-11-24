using Blogapp.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Blogapp.Services
{
    public class MongoBlogService
    {
        private readonly IMongoCollection<BlogPost> _blogCollection;

        public MongoBlogService(IConfiguration config)
        {
            try
            {
                var connectionString = config["MongoDb:ConnectionString"];
                var databaseName = config["MongoDb:DatabaseName"];
                var collectionName = config["MongoDb:CollectionName"];

                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);

                _blogCollection = database.GetCollection<BlogPost>(collectionName);

                Console.WriteLine("MongoDB Connected Successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MongoDB Connection Error: {ex.Message}");
                throw;
            }
        }

        // CRUD OPERATIONS

        public async Task<List<BlogPost>> GetBlogsAsync() =>
            await _blogCollection.Find(_ => true).ToListAsync();

        public async Task AddBlogAsync(BlogPost newBlog)
        {
            await _blogCollection.InsertOneAsync(newBlog);
        }

        public async Task DeleteBlogAsync(string id) =>
            await _blogCollection.DeleteOneAsync(blog => blog.Id == id);

        public async Task UpdateBlogAsync(string id, BlogPost updatedBlog)
        {
            var filter = Builders<BlogPost>.Filter.Eq(b => b.Id, id);
            await _blogCollection.ReplaceOneAsync(filter, updatedBlog);
        }

        // EXTRA FEATURES

        public async Task IncrementLikeAsync(string id)
        {
            var filter = Builders<BlogPost>.Filter.Eq(b => b.Id, id);
            var update = Builders<BlogPost>.Update.Inc(b => b.Likes, 1);
            await _blogCollection.UpdateOneAsync(filter, update);
        }

        public async Task AddCommentAsync(string id, string comment)
        {
            var filter = Builders<BlogPost>.Filter.Eq(b => b.Id, id);
            var update = Builders<BlogPost>.Update.Push(b => b.Comments, comment);
            await _blogCollection.UpdateOneAsync(filter, update);
        }
    }
}
