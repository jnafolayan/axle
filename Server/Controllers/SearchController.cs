using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Axle.Engine;

namespace Axle.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase 
    {

        [HttpGet(SearchResultItem.baseRoute)]
        public IEnumerable<SearchResultItem> Get()
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
}