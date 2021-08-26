using System;
using System.IO;
using System.Threading.Tasks;
using Axle.Engine;
using Microsoft.AspNetCore.Mvc;

namespace Axle.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadFolderController : ControllerBase
    {

        private SearchEngine _engine;

        public UploadFolderController(SearchEngine engine){
            _engine = engine;
        }

        [HttpPost]
        public async Task<ActionResult<UploadResponse>> UploadFolder([FromBody] UploadFolderQuery uploadFolderQuery)
        {
            // Make sure the directory exists
            // Recusively add the documents in the directory to the engine
            if (Directory.Exists(uploadFolderQuery.Path))
            {
                string[] files;
                try
                {
                    files = Directory.GetFiles(uploadFolderQuery.Path, "*", SearchOption.AllDirectories);
                } catch(System.UnauthorizedAccessException)
                {
                    return BadRequest(new UploadResponse{
                        Status = "FOLDER_ACCESS_DENIED",
                        Message = "No permission to access specified folder path",
                    });
                }

                foreach (String file in files)
                {
                    string filename = Path.GetFileNameWithoutExtension(file);
                    string ext = Path.GetExtension(file);
                    if (ext.Length > 0 && _engine.CanParseDocumentType(ext.Substring(1))){
                        Console.WriteLine("Adding", filename);
                        await _engine.AddDocument(file, filename);
                    }
                }

                return Ok(new UploadResponse{
                    Status = "DIRECTORY_FILES_ADDED",
                    Message = "Parsable files in the directory have been added",
                });
            }

            return BadRequest(new UploadResponse{
                Status = "DIRECTORY_NOT_FOUND",
                Message = "Couldn't find directory at specified path"
            });

        }
    }
}