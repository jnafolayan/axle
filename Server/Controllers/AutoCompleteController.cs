using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Axle.Engine;

namespace Axle.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutoCompleteController : ControllerBase
    {
        private SearchEngine _engine;
        public AutoCompleteController(SearchEngine engine)
        {
            _engine = engine;
        }
        [HttpGet]
        public ActionResult<AutoCompleteResult> Autocomplete([FromQuery] SearchQuery searchQuery)
        {
            if (searchQuery is null || searchQuery.Query is null)
                return BadRequest();

            List<string> suggestions = _engine.AutoComplete(searchQuery.Query);

            return new AutoCompleteResult
            {
                Query = searchQuery.Query,
                Suggestions = suggestions
            };

        }
    }
}