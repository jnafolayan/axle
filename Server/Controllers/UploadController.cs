using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Axle.Server.Controllers
{
    /// <summary>
    /// Controller for resource upload
    /// </summary>

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
        public async Task<ActionResult<UploadResponse>> Upload([FromForm] UploadInput uploadInput)
        {
            // Make sure type is valid
            if (uploadInput.Type is null)
                return BadRequest(createResponse(
                    "MISSING_TYPE", 
                    "No upload type found"
                ));

            string uploadType = uploadInput.Type.ToLower();
            if (Array.IndexOf(validTypes, uploadType) == -1)
                return BadRequest(createResponse(
                    "INVALID_TYPE", 
                    "Type should be one of: document, url or sitemap"
                ));

            // Make sure type requirements are met
            UploadResponse validationResponse = ValidateUploadInput(uploadType, uploadInput);
            if (validationResponse != null)
                return BadRequest(validationResponse);

            // TODO: If type is web url, download the associated file content
            
            // Save the document if one exists and get the details
            // documentDetails[0] = path
            // documentDetails[1] = extension (currently based on the file name)
            string[] documentDetails;
            if (IsValidDocument(uploadInput.Document))
                documentDetails = await saveDocumentToDisk(uploadInput.Document);

            return Ok(createResponse(
                "SUCCESS", 
                "Resource uploaded successfully"
            ));
        }
        private UploadResponse ValidateUploadInput(string uploadType, UploadInput uploadInput)
        {
            if (uploadType == "document")
                if (!IsValidDocument(uploadInput.Document))
                    return createResponse(
                        "TYPE_REQUIRMENT_MISSING", 
                        "Upload type 'document' requires you to set the 'document' field"
                    );

            if (uploadType == "url")
                if (!IsValidLink(uploadInput.Link))
                    return createResponse(
                        "TYPE_REQUIREMENT_MISSING", 
                        "Upload type 'url' requires you to set 'link' field"
                    );
            
            if (uploadType == "sitemap")
                if (!IsValidLink(uploadInput.Link) || !IsValidDocument(uploadInput.Document))
                    return createResponse(
                        "TYPE_REQUIREMENT_MISSING", 
                        "Upload type 'sitemap' requires you to set either 'document' or 'link' field"
                    );

            return null;
        }
        private async Task<string[]> saveDocumentToDisk(IFormFile document)
        {
            string uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            string filePath = Path.Combine(uploadDir, document.FileName);

            if (document.Length > 0)
            {
                using (Stream fileStream = new FileStream(filePath, FileMode.Create)) {
                    await document.CopyToAsync(fileStream);
                }
            }

            FileInfo fileInfo = new FileInfo(filePath); 
            return new string[]{ filePath, fileInfo.Extension };
        }

        private UploadResponse createResponse(string status, string message)
        {
            return new UploadResponse
            {
                Status = status,
                Message = message
            };
        }
        private bool IsValidLink (string link)
        {
            if (link is null)
                return false;
            
            Uri uri;
            return Uri.TryCreate(link, UriKind.Absolute, out uri) &&
                    ( uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps );
        }
        private bool IsValidDocument (IFormFile file)
        {
            return file != null;
        }

    }
}