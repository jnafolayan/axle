using System;
using System.IO;
using System.Threading.Tasks;
using Axle.Engine.FileParsers.Exceptions;
using GroupDocs.Parser;

namespace Axle.Engine.FileParsers
{
    /// <summary>
    /// This file parser can parse any kind of file supported by 
    /// GroupDocs.Parser. It is used as a fallback parser for 
    /// extensions we haven't implemented.
    /// </summary>
    public class AlmightyFileParser : FileParserBase
    {
        /// <summary>
        /// Parses a local file into text
        /// </summary>
        /// <param name="filepath">The path to the file</param>
        /// <returns>The text contained in the file</returns>
        public override string ParseLocalFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            using (Parser parser = new Parser(filePath))
            {
                if (!parser.Features.Text)
                {
                    throw new TextExtractionNotSupportedException(filePath);
                }

                using (StreamReader reader = File.OpenText(filePath))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}