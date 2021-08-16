using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Axle.Engine;

namespace Axle.Server.Controllers
{
    public class SearchQuery
    {
        public string Query { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase 
    {

        [HttpPost]
        public ActionResult<IEnumerable<SearchResultItem>> Search(SearchQuery searchQuery)
        {
            // Query should be passed into engine search function
            // Engine should return a List of SearchResultItem

            // Make sure a query is present
            if (searchQuery is null || searchQuery.Query is null)
                return BadRequest();

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
}