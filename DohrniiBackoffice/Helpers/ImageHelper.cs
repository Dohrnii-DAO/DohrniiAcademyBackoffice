using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DohrniiBackoffice.Helpers
{
    public class ImageHelper: IImageHelper
    {
        private readonly IFileProvider _fileProvider;
        private readonly IHostEnvironment _hostingEnvironment;

        public ImageHelper(IFileProvider fileProvider, IHostEnvironment hostingEnvironment)
        {
            _fileProvider = fileProvider;
            _hostingEnvironment = hostingEnvironment;
        }
        #region Local
        public async Task<string> UploadImage(IFormFile file, string userName, string baseUrl="")
        {
            var imgUrl = "";
            if (file != null)
            {
                FileInfo fi = new FileInfo(file.FileName);
                string picName = userName.Replace(".","_") + "_" + string.Format("{0:d}", (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;

                var webPath = _hostingEnvironment.ContentRootPath;
                var path = Path.Combine("", webPath + @"\wwwroot\ImageFiles\" + picName);

                imgUrl = @"/ImageFiles/" + picName;
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            return $"{baseUrl}{imgUrl}";
        }
        #endregion
    }
}