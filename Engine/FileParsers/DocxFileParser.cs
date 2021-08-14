using System.IO;
using DocumentFormat.OpenXml.Packaging;

namespace Axle.Engine.FileParsers{
    public class DocxFileParser{
        public string ParseLocalFile(string filePath){
            if (!File.Exists(filePath)){
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            using (WordprocessingDocument docxFile = WordprocessingDocument.Open(filePath, false)){
                string content = docxFile.MainDocumentPart.Document.Body.InnerText;
                return content;
            }
        }
    }
}