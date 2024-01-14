using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
namespace BJ.Quartz.Quartz
{
    public class TestCache : MemoryCache
    {
        public TestCache(IOptions<MemoryCacheOptions> optionsAccessor) : base(optionsAccessor)
        {
           
        }
        ~TestCache()
        {

        }

    }
}
