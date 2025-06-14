using SampleDotnetApp.Models.DataModels;
using SampleDotnetApp.Services.Interfaces;

namespace SampleDotnetApp.Services
{
    public class BasicDataCacheService : IDataCacheService<DataCacheDataModel>
    {
        private readonly IDictionary<string, DataCacheDataModel> dataCache = new Dictionary<string, DataCacheDataModel>();
        
        public IDictionary<string, bool> AddToCache(IEnumerable<DataCacheDataModel> dataCacheDataModelItemList)
        {
            IDictionary<string, bool> successData = new Dictionary<string, bool>();
            foreach(var dataCacheDataModelItem in dataCacheDataModelItemList)
            {
                if(dataCacheDataModelItem != null && !string.IsNullOrWhiteSpace(dataCacheDataModelItem.Email))
                {
                    if(dataCache.TryAdd(dataCacheDataModelItem.Email, dataCacheDataModelItem))
                    {
                        successData.TryAdd(dataCacheDataModelItem.Email, true);
                    }
                    else
                    {
                        successData.TryAdd(dataCacheDataModelItem.Email, false);
                    }
                }
            }
            return successData;
        }

        public bool AddToCache(DataCacheDataModel dataCacheDataModelItem)
        {
            if(dataCacheDataModelItem != null && !string.IsNullOrWhiteSpace(dataCacheDataModelItem.Email))
            {
                return dataCache.TryAdd(dataCacheDataModelItem.Email, dataCacheDataModelItem);
            }
            return false;
        }

        public DataCacheDataModel GetFromCache(string key)
        {
            if(dataCache.TryGetValue(key, out var dataCacheDataModelItem))
            {
                return dataCacheDataModelItem;
            }
            return null;
        }

        public IDictionary<string, bool> RemoveFromCache(IEnumerable<string> keys)
        {
            IDictionary<string, bool> successData = new Dictionary<string, bool>();
            foreach(var key in keys)
            {
                if(dataCache.ContainsKey(key))
                {
                    successData.TryAdd(key, dataCache.Remove(key));
                }
                else
                {
                    successData.TryAdd(key, false);
                }
            }
            return successData;
        }

        public bool RemoveFromCache(string key)
        {
            if(dataCache.ContainsKey(key))
            {
                return dataCache.Remove(key);
            }
            return false;
        }
    }
}