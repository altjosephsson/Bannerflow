using Bannerflow.Api.Models;
using MongoDB.Driver;

namespace Bannerflow.Api.Data
{
    public interface IMongoDbContext
    {
        IMongoCollection<Banner> Banners { get; }
    }

}
