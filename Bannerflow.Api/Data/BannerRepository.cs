using System.Threading.Tasks;
using Bannerflow.Api.Models;
using Microsoft.Extensions.Options;

namespace Bannerflow.Api.Data
{
    public class BannerRepository : IBannerRepository
    {
        private readonly DbContext _context;

        public BannerRepository(IOptions<DbSettings> settings)
        {
            _context = new DbContext(settings);
        }

        public Task Add(Banner banner)
        {
            throw new System.NotImplementedException();
        }

        public Task<Banner> Get(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Remove(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Update(string id, string body)
        {
            throw new System.NotImplementedException();
        }
    }
}
