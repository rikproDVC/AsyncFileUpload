using System.Data.Entity;

namespace AspNet.AzureStorage.Sample.Models
{
    public class FileStorageSampleContext : DbContext
    {
        public DbSet<File> Files { get; set; }
    }
}