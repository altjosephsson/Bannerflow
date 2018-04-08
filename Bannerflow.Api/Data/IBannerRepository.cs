using Bannerflow.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bannerflow.Api.Data
{
    public interface IBannerRepository
    {
        Task<Banner> GetAsync(Guid id);

        // add new note document
        Task<Banner> AddAsync(Banner banner);

        // remove a single document / note
        Task<bool> RemoveAsync(Guid id);

        // update just a single document / note
        Task<bool> UpdateAsync(Guid id, Banner banner);

        // for development purpose
        Task<bool> RemoveAllBannersAsync();

        Task<string> CreateIndexAsync();

        Task<IEnumerable<Banner>> GetAllAsync();        
    }
}
