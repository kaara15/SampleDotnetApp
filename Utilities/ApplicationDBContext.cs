using Microsoft.EntityFrameworkCore;

namespace SampleDotnetApp.Utilities
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }
        
        public DbSet<Models.DataModels.DataCacheDataModel> DataCacheDataModels { get; set; }
    }    
}