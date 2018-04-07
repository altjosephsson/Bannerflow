using Bannerflow.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Bannerflow.Api.Data
{
    public class DbContext
    {
        private readonly IMongoDatabase _database;

        public DbContext(IOptions<DbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
            {
                _database = client.GetDatabase(settings.Value.Database);
            }
               
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
