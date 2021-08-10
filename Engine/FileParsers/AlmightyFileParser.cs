using System;
using GroupDocs.Parser;

namespace Axle.Engine.FileParsers
{
    public class AlmightyFileParser : FileParserBase
    {
        /// <summary>
        /// Parses a local file into text
        /// </summary>
        /// <param name="filepath">The path to the file</param>
        /// <returns>The text contained in the file</returns>
        public override string ParseLocalFile(string filePath)
        {
            return "";
        }

        /// <summary>
        /// Parses a remote file into text
        /// </summary>
        /// <param name="fileURL">The url to the file</param>
        /// <returns>The text contained in the file</returns>
        public override string ParseRemoteFile(string fileURL)
        {
            return "";
        }
    }
}