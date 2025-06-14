using SampleDotnetApp.Services;
using SampleDotnetApp.Services.Interfaces;
using SampleDotnetApp.Utilities.Interfaces;
using SampleDotnetApp.Utilities.Settings;
using SampleDotnetApp.Models.DataModels;

namespace SampleDotnetApp.Utilities
{
    public class CacheFactory : ICacheFactory<DataCacheDataModel>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ApplicationSettings applicationSettings;

        public CacheFactory(IServiceProvider serviceProvider, ApplicationSettings applicationSettings)
        {
            this.serviceProvider = serviceProvider;
            this.applicationSettings = applicationSettings;
        }

        public IDataCacheService<DataCacheDataModel> GetDefaultCache()
        {
            switch(applicationSettings.CacheSettings?.Type)
            {
                case "Basic":
                    return serviceProvider.GetRequiredService<BasicDataCacheService>();
                case "RTWT":
                    return serviceProvider.GetRequiredService<RTWTCacheService>();
                default:
                    throw new NotImplementedException($"Cache type '{applicationSettings.CacheSettings?.Type}' is not implemented.");
            }
        }

        public IDataCacheService<DataCacheDataModel> GetCache(string type)
        {
            switch(type)
            {
                case "Basic" :
                    return serviceProvider.GetRequiredService<BasicDataCacheService>();
                case "RTWT" :
                    return serviceProvider.GetRequiredService<RTWTCacheService>();
                default :
                    throw new NotImplementedException($"Cache type '{type}' is not implemented.");
            }
        }
    }
}