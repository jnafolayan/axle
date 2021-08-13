using System;
using System.Collections.Generic;

namespace Axle.Engine.FileParsers
{
    public class FileParserFactory
    {
        private Dictionary<string, FileParserBase> parsers;

        public FileParserFactory()
        {
            parsers = new Dictionary<string, FileParserBase>();
        }

        public void RegisterParser(string ext, FileParserBase parser)
        {
            if (parsers.ContainsKey(ext))
            {
                throw new Exception($"Cannot register multiple parsers for '{ext}'");
            }

            parsers.Add(ext, parser);
        }

        public FileParserBase GetParser(string ext)
        {
            if (parsers.ContainsKey(ext))
            {
                return parsers[ext];
            }

            return null;
        }
    }
}