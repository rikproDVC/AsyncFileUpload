using AspNet.AzureStorage.Sample.Models;
using AspNet.AzureStorage.Sample.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace AspNet.AzureStorage.Sample.Controllers
{
    public class FileController : Controller
    {
        // GET: File
        public ActionResult Index()
        {
            var files = _ctx.Files.ToList();

            return View(files);
        }

        public ActionResult Upload()
        {
            return View(new FileUploadViewModel());
        }

        [HttpPost]
        public async Task<JsonResult> UploadAjax()
        {
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var uploader = new FileUploader(fileContent, "files");
                        var uploadedFile = await uploader.Upload();
                    }
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            return Json("File uploaded successfully");
        }

        private readonly FileStorageSampleContext _ctx = new FileStorageSampleContext();
    }
}