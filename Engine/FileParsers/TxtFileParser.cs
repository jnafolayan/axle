using System.IO;

namespace Axle.Engine.FileParsers
{
    /// <summary>
    /// Parser for TXT files
    /// </summary>
    public class TxtFileParser : FileParserBase
    {
        /// <summary>
        /// Parses a TXT file and returns the contents as a string
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>The text contained in the file as a string</returns>
        public override string ParseLocalFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            string text = File.ReadAllText(filePath);
            return text;
        }
    }
}