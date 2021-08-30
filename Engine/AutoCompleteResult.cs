using System.Collections.Generic;

namespace Axle.Engine
{
    public class AutoCompleteResult
    {
        public string Query { get; set; }
        public List<string> Suggestions { get; set; }
    }
}