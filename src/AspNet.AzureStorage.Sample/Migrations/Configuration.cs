using System.Data.Entity.Migrations;

namespace AspNet.AzureStorage.Sample.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AspNet.AzureStorage.Sample.Models.FileStorageSampleContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(AspNet.AzureStorage.Sample.Models.FileStorageSampleContext context)
        {

        }
    }
}