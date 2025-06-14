using SampleDotnetApp.Models.DataModels;

namespace SampleDotnetApp.Services.Interfaces
{
    public interface IDataCacheService<T> where T : class
    {
        public IDictionary<string, bool> AddToCache(IEnumerable<T> dataCacheDataModelItemList);
        public bool AddToCache(T dataCacheDataModelItem);
        public T GetFromCache(string key);
        public IDictionary<string, bool> RemoveFromCache(IEnumerable<string> keys);
        public bool RemoveFromCache(string key);
    }
}