using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AspNet.AzureStorage.Sample.ViewModels
{
    public class FileUploadViewModel
    {
        [Required]
        [DataType(DataType.Upload)]
        [ImageSize(2000000)]
        [IsMimeTypes(new string[] { "image" })]
        public HttpPostedFileBase UploadFile { get; set; }
    }
}