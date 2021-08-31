using System;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Axle.Engine;
using System.Text;
using System.Text.RegularExpressions;
using MimeTypes;
using Axle.Server.Controllers;

namespace Axle.Server.Controllers
{
    /// <summary>
    /// Controller for resource upload
    /// </summary>

    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {

        private SearchEngine _engine;
        private IWebHostEnvironment _hostingEnvironment;
        private WebClient _client;

        public UploadController(IWebHostEnvironment env, SearchEngine engine)
        {
            _hostingEnvironment = env;
            _engine = engine;
            _client = new WebClient();
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

            // Save the document if one exists and get the details
            List<UploadError> errors = new List<UploadError>();
            if (uploadType == "document")
                errors = await UploadDocuments(uploadInput, errors);
            else if (uploadType == "url")
                errors = await UploadWebUrl(uploadInput.Link, errors);
            else if (uploadType == "sitemap")
                errors = await UploadSitemap(uploadInput, errors);

            if (errors.Count > 0)
            {
                return Ok(createResponse(
                    "PARTIAL_SUCCESS",
                    "Some resources failed to upload",
                    errors
                ));
            }

            return Ok(createResponse(
                "SUCCESS",
                "Resource uploaded successfully"
            ));
        }
        private UploadResponse ValidateUploadInput(string uploadType, UploadInput uploadInput)
        {
            if (uploadType == "document")
                if (!IsValidDocuments(uploadInput.Documents))
                    return createResponse(
                        "TYPE_REQUIRMENT_MISSING",
                        "Upload type 'document' requires you to set the 'documents' field"
                    );

            if (uploadType == "url")
                if (!IsValidLink(uploadInput.Link))
                    return createResponse(
                        "TYPE_REQUIREMENT_MISSING",
                        "Upload type 'url' requires you to set 'link' field"
                    );

            if (uploadType == "sitemap")
                if (!IsValidDocuments(uploadInput.Documents))
                    return createResponse(
                        "TYPE_REQUIREMENT_MISSING",
                        "Upload type 'sitemap' requires you to set the 'documents' field"
                    );

            return null;
        }

        private async Task<List<UploadError>> UploadDocuments(UploadInput uploadInput, List<UploadError> errors)
        {
            if (IsValidDocuments(uploadInput.Documents))
            {
                foreach (IFormFile document in uploadInput.Documents)
                {
                    // Ensure we can parse the document
                    if (!CanParseDocument(document))
                    {
                        errors.Add(new UploadError
                        {
                            Status = "UNPROCESSABLE_DOCUMENT",
                            Message = "Cannot parse document"
                        });
                        continue;
                    }

                    await saveDocumentToDisk(document, uploadInput.Title, uploadInput.Description);
                }
            }
            return errors;
        }

        private async Task<List<UploadError>> UploadWebUrl(string link, List<UploadError> errors)
        {
            Console.WriteLine("I have been called");
            // Get the extension of the link
            // if the link type is parsable, then you download the content and add to db
            // else you ignore it and return the unprocessable document error

            WebRequest request = HttpWebRequest.Create(link);
            request.Method = "HEAD";

            string mimetype = request.GetResponse().ContentType;
            mimetype = mimetype.Split(";")[0];

            string extension = MimeTypeMap.GetExtension(mimetype, false);
            extension = extension.Substring(1);

            if (!_engine.CanParseDocumentType(extension))
            {
                errors.Add(new UploadError
                {
                    Status = "UNPROCESSABLE_DOCUMENT",
                    Message = "Cannot parse document"
                });
                return errors;
            }

            string fileName = Utils.GenerateGUID(11) + "." + extension;
            string filePath = Path.GetFullPath("./wwwroot/uploads/" + fileName);
            _client.DownloadFile(new Uri(link), "./wwwroot/uploads/" + fileName);
            await _engine.AddDocument(filePath, Path.GetFileNameWithoutExtension(link), "");

            return errors;
        }

        // public static async Task<List<string>> ReadAsStringAsync(this IFormFile file)
        // {
        //     var result = new List<string>();
        //     using (var reader = new StreamReader(file.OpenReadStream()))
        //     {
        //         while (reader.Peek() >= 0)
        //             result.Add(await reader.ReadLineAsync());
        //     }
        //     return result;
        // }

        // public static List<string> ReadAsList(this IFormFile file)
        // {
        //     var result = new List<string>();
        //     using (var reader = new StreamReader(file.OpenReadStream()))
        //     {
        //         while (reader.Peek() >= 0)
        //             result.Add(reader.ReadLine());
        //     }
        //     return result;
        // }

        public static async Task<List<string>> ReadFormFileAsync(IFormFile file)
        {
            var resultList = new List<string>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    line = line.Trim();
                    resultList.Add(line);
                }
            }
            return resultList;
        }

        private async Task<List<UploadError>> UploadSitemap(UploadInput uploadInput, List<UploadError> errors)
        {
            // IFormFile sitemapDoc = uploadInput.Documents[0];
            // await saveDocumentToDisk(sitemapDoc, uploadInput.Title, uploadInput.Description);
            var file = uploadInput.Documents[0];
            // List<string> Index(IFormFile file) => file.ReadAsStringAsync();
            List<string> urlsList = await ReadFormFileAsync(file);
            Utils.RunTasks<string>(urlsList, 5, (url) => {
                Console.WriteLine("Lambda to the rescue");
                return UploadWebUrl(url, errors);
            });

            return errors;
        }


        private async Task<string[]> saveDocumentToDisk(IFormFile document, String title, String description)
        {
            string guid = Utils.GenerateGUID(11);
            var extension = Path.GetExtension(document.FileName);
            var newFileName = Regex.Replace(document.FileName, @"\.\S+$", "__" + guid + extension); // name_guid.ext
            string uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            string filePath = Path.Combine(uploadDir, newFileName);

            if (title is null)
                title = Path.GetFileNameWithoutExtension(document.FileName);

            if (description is null)
                description = "";

            if (document.Length > 0)
            {
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await document.CopyToAsync(fileStream);
                    await _engine.AddDocument(filePath, title, description);
                }
            }

            var fileInfo = new FileInfo(filePath);
            return new string[] { filePath, fileInfo.Extension.Substring(1) };
        }

        private bool CanParseDocument(IFormFile document)
        {
            var ext = Path.GetExtension(document.FileName).Substring(1);
            return _engine.CanParseDocumentType(ext);
        }

        private UploadResponse createResponse(string status, string message, List<UploadError> errors = null)
        {
            return new UploadResponse
            {
                Status = status,
                Message = message,
                Errors = errors,
            };
        }
        private bool IsValidLink(string link)
        {
            if (link is null)
                return false;

            Uri uri;
            return Uri.TryCreate(link, UriKind.Absolute, out uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
        private bool IsValidDocuments(IFormFile[] documents)
        {
            if (documents is null)
                return false;

            return documents.Length > 0;
        }

    }
}