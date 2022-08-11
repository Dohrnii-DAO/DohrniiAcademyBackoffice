using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DohrniiBackoffice.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImage(IFormFile file, string userName, string baseUrl = "");
    }
}
