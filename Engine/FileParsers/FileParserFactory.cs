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

        /// <summary>
        /// Registers a new parser for an extension
        /// </summary>
        /// <param name="ext">The file extension</param>
        /// <param name="parser">An instance of a parser</param>
        public void RegisterParser(string ext, FileParserBase parser)
        {
            if (parsers.ContainsKey(ext))
            {
                throw new Exception($"Cannot register multiple parsers for '{ext}'");
            }

            parsers.Add(ext, parser);
        }

        /// <summary>
        /// Gets the parser registered for an extension
        /// </summary>
        /// <param name="ext"></param>
        /// <returns>The parser instance</returns>
        public FileParserBase GetParser(string ext)
        {
            if (parsers.ContainsKey(ext))
            {
                return parsers[ext];
            }

            return null;
        }

        /// <summary>
        /// Gets all file extensions that are supported
        /// </summary>
        /// <returns>A list of all extensions</returns>
        public List<string> GetSupportedFileExtensions()
        {
            var keys = parsers.Keys;
            var exts = new List<string>();
            
            foreach (var key in keys)
            {
                exts.Add(key);
            }

            return exts;
        }
    }
}