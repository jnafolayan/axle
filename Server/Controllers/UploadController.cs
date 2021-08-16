using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace Axle.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {

        private IWebHostEnvironment _hostingEnvironment;
        
        public UploadController(IWebHostEnvironment env)
        {
            _hostingEnvironment = env;
        }

        private string[] validTypes = new string[]{
            "document",
            "url",
            "sitemap",
        };

        [HttpPost]
        public ActionResult<UploadResponse> Upload([FromForm] UploadInput uploadInput)
        {
            // Make sure type is valid
            if (uploadInput.Type is null)
                return new UploadResponse{
                    Status = "INVALID_TYPE"
                };

            string uploadType = uploadInput.Type.ToLower();
            if (Array.IndexOf(validTypes, uploadType) == -1)
                return new UploadResponse{
                    Status = "INVALID_TYPE"
                };

            // Make sure type requirements are met
            UploadResponse validationResponse = ValidateUploadInput(uploadType, uploadInput);
            if (validationResponse != null)
                return validationResponse;

            // Save the document
            if (IsValidDocument(uploadInput.Document))
                saveDocumentToDisk(uploadInput.Document);

            // If type is document, store document in physical location
            // Figure out common output interface and expose it for db
            return new UploadResponse{
                Status = "success"
            };
        }
        private bool IsValidLink (string link)
        {
            return true;
        }
        private bool IsValidDocument (IFormFile file)
        {
            return true;
        }
        private UploadResponse ValidateUploadInput(string uploadType, UploadInput uploadInput)
        {
            if (uploadType == "document")
                if (uploadInput.Document is null)
                    return new UploadResponse{
                        Status = "DOCUMENT_TYPE_NEEDS_DOCUMENT"
                    };

            if (uploadType == "url")
                if (IsValidLink(uploadInput.Link))
                    return new UploadResponse{
                        Status = "URL_TYPE_NEEDS_LINK"
                    };
            
            if (uploadType == "sitemap")
                if (IsValidLink(uploadInput.Link) || IsValidDocument(uploadInput.Document))
                    return new UploadResponse{
                        Status = "SITEMAP_TYPE_NEEDS_VALID_DOCUMENT_OR_LINK"
                    };

            return null;
        }
        private async void saveDocumentToDisk(IFormFile document)
        {
            Console.WriteLine("env", _hostingEnvironment.WebRootPath);
            string uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            if (document.Length > 0)
            {
                string filePath = Path.Combine(uploadDir, document.FileName); // convert name to id
                using (Stream fileStream = new FileStream(filePath, FileMode.Create)) {
                    await document.CopyToAsync(fileStream);
                }
            }
        }

    }
}