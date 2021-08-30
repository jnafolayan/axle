using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Axle.Engine;

namespace Axle.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndexDocumentController : ControllerBase
    {
        private SearchEngine _engine;
        public IndexDocumentController(SearchEngine engine)
        {
            _engine = engine;
        }

        [HttpGet]
        public ActionResult IndexDocument()
        {
            _engine.IndexAllDocuments();
            return Ok();
        }
    }
}