using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Axle.Engine;

namespace Axle.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutoCompleteController : ControllerBase
    {
        [HttpPost]
        public ActionResult<AutoCompleteResult> Autocomplete(SearchQuery searchQuery)
        {
            if (searchQuery is null || searchQuery.Query is null)
                return BadRequest();

            return new AutoCompleteResult{
                Query = searchQuery.Query,
                Suggestions = new string[]{
                    "suggestion1",
                    "suggestion2"
                }
            };

        }
    }
}