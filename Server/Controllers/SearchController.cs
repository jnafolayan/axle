using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Axle.Engine;

namespace Axle.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase 
    {

        [HttpPost]
        public ActionResult<IEnumerable<SearchResultItem>> Search(SearchQuery searchQuery)
        {
            return new List<SearchResultItem>{
                new SearchResultItem{
                    Title = "Result 1",
                    Description = "First result",
                    Link = "https://1.com",
                },
                new SearchResultItem{
                    Title = "Result 2",
                    Description = "Second result",
                    Link = "https://2.com",
                }
            };
        }
    }

    public class SearchQuery
    {
        public string Query { get; }
    }
}