using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AspNet.AzureStorage.Sample
{
    class ImageSizeAttribute : ValidationAttribute
    {
        public int MaxSize { get; set; }

        public ImageSizeAttribute(int maxSize)
        {
            MaxSize = 2000000;
        }

        public override bool IsValid(object value)
        {
            HttpPostedFileBase file = value as HttpPostedFileBase;
            if (file != null)
            {
                return file.ContentLength <= MaxSize;
            }

            return true;
        }
    }

    class IsMimeTypesAttribute : ValidationAttribute
    {
        public string[] Mimes { get; set; }

        public IsMimeTypesAttribute(string[] mimes)
        {
            Mimes = mimes.Any() ? mimes : new string[] { };
        }

        public override bool IsValid(object value)
        {
            HttpPostedFileBase file = value as HttpPostedFileBase;
            if (file != null && Mimes.Any())
            {
                return Mimes.All(mime => file.ContentType.Contains(mime));
            }

            return true;
        }
    }
}