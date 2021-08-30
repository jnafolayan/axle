using Microsoft.AspNetCore.Mvc;
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
        public void IndexDocument()
        {
            _engine.IndexAllDocuments()
        }
    }
}