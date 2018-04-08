using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bannerflow.Api.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Bannerflow.Api.Data
{
    public class BannerRepository : IBannerRepository
    {
        private readonly IMongoDbContext _context;
        private readonly ILogger<BannerRepository> _logger;

        public BannerRepository(ILogger<BannerRepository> logger, IMongoDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Banner> AddAsync(Banner banner)
        {
            try
            {
                await _context.Banners.InsertOneAsync(banner);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return banner;
        }

        public async Task<Banner> GetAsync(Guid id)
        {
            try
            {
                return await _context.Banners
                                .Find(banner => banner.Id == id)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            try
            {               
                DeleteResult actionResult = await _context.Banners.DeleteOneAsync(
                     Builders<Banner>.Filter.Eq("Id", id));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateAsync(Guid id, Banner banner)
        {           
            try
            {
                ReplaceOneResult actionResult = await _context.Banners
                                                .ReplaceOneAsync(n => n.Id.Equals(id)
                                                                , banner
                                                                , new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<bool> RemoveAllBannersAsync()
        {
            try
            {
                DeleteResult actionResult = await _context.Banners.DeleteManyAsync(new BsonDocument());

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<string> CreateIndexAsync()
        {
            try
            {
                return await _context.Banners.Indexes
                                           .CreateOneAsync(Builders<Banner>
                                                                .IndexKeys
                                                                .Ascending(item => item.Id)
                                                                .Ascending(item => item.Created));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Banner>> GetAllAsync()
        {
            try
            {
                return await _context.Banners.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
