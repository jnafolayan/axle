using System;
using System.IO;
using DocumentFormat.OpenXml.Packaging;

namespace Axle.Engine.FileParsers
{
    /// <summary>
    /// Parser for Word Documents
    /// </summary>
    public class WordDocumentParser : FileParserBase
    {
        /// <summary>
        /// Parses a .docx file into text
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>The text contained in the file</returns>
        public override string ParseLocalFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            using (WordprocessingDocument docxFile = WordprocessingDocument.Open(filePath, false))
            {
                string content = docxFile.MainDocumentPart.Document.Body.InnerText;
                return content;
            }
        }
    }
}