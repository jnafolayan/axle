using Microsoft.AspNetCore.Http;

namespace Axle.Server.Controllers
{
    // Types to deserialize json
    public class SearchQuery
    {
        public string Query { get; set; }
    }

    public class UploadInput
    {
        public string Type { get; set; }
        public IFormFile Document { get; set; }
        public string Link { get; set; }
    }

    public class UploadResponse
    {
        public string Status { get; set; }
    }
}