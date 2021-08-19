using System.Collections.Generic;

namespace Axle.Engine
{
    public class AutoCompleteResult
    {
        public string Query { get; set; }
        public string[] Suggestions { get; set; }
    }
}