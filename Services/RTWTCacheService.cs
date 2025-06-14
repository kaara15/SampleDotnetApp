using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using SampleDotnetApp.Models.DataModels;
using SampleDotnetApp.Services.Interfaces;
using SampleDotnetApp.Utilities;

namespace SampleDotnetApp.Services
{
    public class RTWTCacheService : IDataCacheService<DataCacheDataModel>
    {
        private readonly IServiceProvider serviceProvider;
        
        public RTWTCacheService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

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
                        using IServiceScope scope = serviceProvider.CreateScope();
                        ApplicationDBContext applicationDBContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                        applicationDBContext.DataCacheDataModels.Add(dataCacheDataModelItem);
                        applicationDBContext.SaveChanges();
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
                if(dataCache.TryAdd(dataCacheDataModelItem.Email, dataCacheDataModelItem))
                {
                    using IServiceScope scope = serviceProvider.CreateScope();
                    ApplicationDBContext applicationDBContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    applicationDBContext.DataCacheDataModels.Add(dataCacheDataModelItem);
                    applicationDBContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public DataCacheDataModel GetFromCache(string key)
        {
            if(dataCache.TryGetValue(key, out DataCacheDataModel? dataCacheDataModel))
            {
                return dataCacheDataModel;
            }
            using IServiceScope scope = serviceProvider.CreateScope();
            ApplicationDBContext applicationDBContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            DataCacheDataModel? dbDataCacheDataModel = applicationDBContext.DataCacheDataModels.Where(dataCacheDataModel => EF.Functions.ILike(dataCacheDataModel.Email, key)).FirstOrDefault();
            if(dbDataCacheDataModel != null)
            {
                dataCache[key] = dbDataCacheDataModel;
            }
            return dbDataCacheDataModel;
        }

        public IDictionary<string, bool> RemoveFromCache(IEnumerable<string> keys)
        {
            IDictionary<string, bool> successData = new Dictionary<string, bool>();
            foreach(var key in keys)
            {
                if(dataCache.TryGetValue(key, out DataCacheDataModel? dataCacheDataModel))
                {
                    using IServiceScope scope = serviceProvider.CreateScope();
                    ApplicationDBContext applicationDBContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    applicationDBContext.DataCacheDataModels.Remove(dataCacheDataModel);
                    applicationDBContext.SaveChanges();
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
            if(dataCache.TryGetValue(key, out DataCacheDataModel? dataCacheDataModel))
            {
                using IServiceScope scope = serviceProvider.CreateScope();
                ApplicationDBContext applicationDBContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                applicationDBContext.DataCacheDataModels.Remove(dataCacheDataModel);
                applicationDBContext.SaveChanges();
                dataCache.Remove(key);
                return true;
            }
            return false;
        }
    }
}