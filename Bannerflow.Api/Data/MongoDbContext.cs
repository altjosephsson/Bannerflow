using Bannerflow.Api.Models;
using MongoDB.Driver;
using System;

namespace Bannerflow.Api.Data
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IMongoDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public IMongoCollection<Banner> Banners
        {
            get
            {
                return _database.GetCollection<Banner>("Banner");
            }
        }
    }

}
