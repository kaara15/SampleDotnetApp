
using SampleDotnetApp.Services.Interfaces;
using SampleDotnetApp.Models.DataModels;

namespace SampleDotnetApp.Utilities.Interfaces
{
    public interface ICacheFactory<T> where T : class
    {
        public IDataCacheService<T> GetDefaultCache();
        public IDataCacheService<T> GetCache(string type);
    }
}