using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Axle.Engine;
using System.Threading.Tasks;
using System.Diagnostics;

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
        public async Task<ActionResult<SearchResult>> Search([FromQuery] SearchQuery searchQuery)
        {

            // Make sure a query is present
            if (searchQuery is null || searchQuery.Query is null)
                return BadRequest();

            var watch = new Stopwatch();
            watch.Start();
            var results = await _engine.ExecuteQuery(searchQuery.Query);
            watch.Stop();
            long elapsed = watch.ElapsedMilliseconds;
            return new SearchResult
            {
                Speed = elapsed,
                Documents = results,
            };
        }
    }

    public class SearchResult
    {
        public long Speed { get; set; }
        public IEnumerable<SearchResultItem> Documents { get; set; }
    }
}