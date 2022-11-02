using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace CBT.BLL.Services.FileUpload
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor accessor;
        private static string PassportPhotoPath = "PassportPhoto";
        public FileUploadService(IWebHostEnvironment environment, IHttpContextAccessor httpContext)
        {
            _environment = environment;
            accessor = httpContext;
        }
        public string UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return "";
                }
                int maxFileSize = 1024 * 1024 / 2;
                var fileSize = file.Length;

                if (fileSize > maxFileSize)
                {
                    throw new ArgumentException($"file limit exceeded, greater than {maxFileSize}");
                }

                if (file.FileName.EndsWith(".jpg")
                            || file != null && file.Length > 0 || file.FileName.EndsWith(".jpg")
                            || file.FileName.EndsWith(".jpeg") || file.FileName.EndsWith(".png"))
                {
                    string extension = Path.GetExtension(file.FileName);
                    string fileName = Guid.NewGuid().ToString() + extension;

                    var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/" + PassportPhotoPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fileStream.Position = 0;
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }

                    var host = accessor.HttpContext.Request.Host.ToUriComponent();
                    var url = $"{accessor.HttpContext.Request.Scheme}://{host}/{PassportPhotoPath}/{fileName}";
                    return url;
                }
                throw new ArgumentException("Invalid Profile Image");
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
