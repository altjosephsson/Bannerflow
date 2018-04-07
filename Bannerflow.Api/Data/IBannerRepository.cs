using Bannerflow.Api.Models;
using System.Threading.Tasks;

namespace Bannerflow.Api.Data
{
    public interface IBannerRepository
    {
        Task<Banner> Get(string id);

        // add new note document
        Task Add(Banner banner);

        // remove a single document / note
        Task<bool> Remove(string id);

        // update just a single document / note
        Task<bool> Update(string id, string body);
    }
}
