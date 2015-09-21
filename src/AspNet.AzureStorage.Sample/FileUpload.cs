using AspNet.AzureStorage.Sample.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;

namespace AspNet.AzureStorage.Sample
{
    public class FileUploader
    {
        public HttpPostedFileBase File { get; set; }
        public string Container { get; set; }

        public FileUploader(HttpPostedFileBase file, string container)
        {
            File = file;
            Container = container;
        }

        public async Task<Models.File> Upload()
        {
            // Set the file stream postion to the start of the stream
            File.InputStream.Position = 0;

            // Create extension, unique key and file hash variables, and reset position after reading stream
            var extension = Path.GetExtension(File.FileName);
            var key = string.Format("File-{0}{1}", Guid.NewGuid(), extension);
            var hash = new MD5Cng().ComputeHash(File.InputStream);
            File.InputStream.Position = 0;

            // Check if the file already exists in the Database using the hash; Return the existing file object if the file exists already
            if (_ctx.Files.Any(m => m.Md5 == hash && m.Container == Container))
            {
                return _ctx.Files.Single(m => m.Md5 == hash && m.Container == Container);
            }

            // Get / create the blob container reference
            _container = _blobClient.GetContainerReference(Container);
            _container.CreateIfNotExists();
            _container.SetPermissions(
                new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

            // Create new database object to reference the blob from
            var fileEntity = new Models.File
            {
                Name = File.FileName.Replace(extension ?? "", ""),
                ContentLength = File.ContentLength,
                ContentType = File.ContentType,
                Key = key,
                Container = Container,
                Md5 = hash  
            };

            _ctx.Files.Add(fileEntity);
            await _ctx.SaveChangesAsync();

            // Upload original file
            var block = _container.GetBlockBlobReference(fileEntity.Key);
            block.Properties.ContentType = fileEntity.ContentType;
            await block.UploadFromStreamAsync(File.InputStream);

            // Upload resized image versions
            await UploadImageResized(fileEntity);

            return fileEntity;
        }

        private async Task UploadImageResized(Models.File file)
        {
            using (var imageStream = File.InputStream)
            {
                foreach (var s in ImageSizes.List)
                {
                    // Only upload additional files if the image is larger than the target size
                    if (ImageHelper.IsLargerThanDimensions(imageStream, Convert.ToInt32(s.Split('/').Last())))
                    {
                        // Format key with size identifier included
                        var key = string.Format("{0}_{1}.{2}",
                        file.Key.Split('.').First(),
                        s.Split('/').First(),
                        file.Key.Split('.').Last());

                        var size = s.Split('/').Last();

                        // Create a new copy of the momerystream. Copying ensures there won't be any stream position issues
                        imageStream.Position = 0;
                        Stream stream = new MemoryStream();
                        imageStream.CopyTo(stream);
                        stream.Position = 0;

                        // Upload the file to the Blob Storage
                        var block = _container.GetBlockBlobReference(key);
                        block.Properties.ContentType = file.ContentType;
                        await block.UploadFromStreamAsync(ImageHelper.ResizeImage(stream, Convert.ToInt32(size)));
                    }
                }
            }
        }

        public bool IsSize(int maxSize)
        {
            return File.ContentLength <= maxSize;
        }

        public bool IsSmallerThanDimensions(Stream imageStream, int maxHeight, int maxWidth)
        {
            using (var img = Image.FromStream(imageStream))
            {
                if (img.Width <= maxWidth && img.Height <= maxHeight) return true;
            }
            return false;
        }

        private static readonly FileStorageSampleContext _ctx = new FileStorageSampleContext();
        private static readonly CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        private static readonly CloudBlobClient _blobClient = _storageAccount.CreateCloudBlobClient();
        private static CloudBlobContainer _container;
    }
}
