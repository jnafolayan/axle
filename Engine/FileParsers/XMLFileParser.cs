using System.Xml;
using System.IO;

namespace Axle.Engine.FileParsers
{
    /// <summary>
    /// Parser for XML files
    /// </summary>
    public class XMLFileParser: FileParserBase
    {
        /// <summary>
        /// Parses an XML file and returns the contents as a string
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>The text contained in the file as a string</returns>
        public override string ParseLocalFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                reader.MoveToContent();
                string content = reader.ReadInnerXml();
                return content;
            }
        }
    }
}

