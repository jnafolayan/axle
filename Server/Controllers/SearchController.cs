using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Axle.Engine;
using System.Threading.Tasks;

namespace Axle.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private SearchEngine _engine;

        public SearchController(SearchEngine engine)
        {
            _engine = engine;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SearchResultItem>>> Search([FromQuery] SearchQuery searchQuery)
        {
            // Query should be passed into engine search function
            // Engine should return a List of SearchResultItem

            // Make sure a query is present
            if (searchQuery is null || searchQuery.Query is null)
                return BadRequest();

            var results = await _engine.ExecuteQuery(searchQuery.Query);
            return results;
        }
    }
}