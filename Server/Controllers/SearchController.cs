using System;
using System.IO;
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

            _engine.IndexAllDocuments();
            // Make sure a query is present
            if (searchQuery is null || searchQuery.Query is null)
                return BadRequest();

            var watch = new Stopwatch();
            watch.Start();
            var results = await _engine.ExecuteQuery(searchQuery.Query);
            watch.Stop();
            long elapsed = watch.ElapsedMilliseconds;

            List<SearchResultItem> newResults = results.ConvertAll<SearchResultItem>((resultItem) => {
                return new SearchResultItem{
                    Title = resultItem.Title,
                    Description = resultItem.Description,
                    Link = extractStaticFileUrl(resultItem.Link)
                };
            });

            return new SearchResult
            {
                Speed = elapsed,
                Documents = newResults,
            };
        }

        public String extractStaticFileUrl(String absoluteUrl)
        {
            String filename = Path.GetFileNameWithoutExtension(absoluteUrl) + Path.GetExtension(absoluteUrl);
            return "/uploads/" + filename;
        }
    }

    public class SearchResult
    {
        public long Speed { get; set; }
        public IEnumerable<SearchResultItem> Documents { get; set; }
    }
}