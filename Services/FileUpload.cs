using Microsoft.AspNetCore.Components.Forms;

namespace BlazorFileUpload.Services
{
    public interface IFileUpload
    {
        Task UploadFile(IBrowserFile file);
        Task<string> GeneratePreviewUrl(IBrowserFile file); 
    }

    public class FileUpload : IFileUpload
    {
        private IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileUpload> _logger;

        public FileUpload(IWebHostEnvironment webHostEnvironment, ILogger<FileUpload> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task UploadFile(IBrowserFile file)
        {
            if (file is not null)
            {
                try
                {
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", file.Name);

                    using (var stream =  file.OpenReadStream())
                    {
                        var fileStream = File.Create(uploadPath);
                        await stream.CopyToAsync(fileStream);
                        fileStream.Close();
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }

        public async Task<string> GeneratePreviewUrl(IBrowserFile file)
        {
            if (!file.ContentType.Contains("image"))
            {
                if (file.ContentType.Contains("pdf"))
                {
                    return "images/pdf_logo.png";
                }
            }

            var resizeImage = await file.RequestImageFileAsync(file.ContentType, 100, 100);
            var buffer = new byte[resizeImage.Size];
            await resizeImage.OpenReadStream().ReadAsync(buffer);
            return $"data:{file.ContentType}; base64{Convert.ToBase64String(buffer)}";
        }
    }
}
