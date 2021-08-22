using System.IO;
using Spire.Doc;
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
            else if (Path.GetExtension(filePath) == ".doc")
            {
                // Convert the file from .doc to .docx
                Document doc = new Document();
                doc.LoadFromFile(filePath);
                filePath = $"{filePath}.docx";
                doc.SaveToFile(filePath, FileFormat.Docx2013);
            }

            using (WordprocessingDocument docxFile = WordprocessingDocument.Open(filePath, false))
            {
                string content = docxFile.MainDocumentPart.Document.Body.InnerText;
                File.Delete(filePath);
                return content;
            }
        }
    }
}