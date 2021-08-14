using System;
using System.IO;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Axle.Engine.FileParsers
{
    /// <summary>
    /// Parser for PDF files
    /// </summary>
    public class PdfFileParser : FileParserBase
    {
        /// <summary>
        /// Parses a PDF file and returns the contents as a string
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>The text contained in the file as a string</returns>
        public override string ParseLocalFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            var pdfDocument = new PdfDocument(new PdfReader(filePath));
            StringBuilder processed = new StringBuilder();
            for (int i = 1; i <= pdfDocument.GetNumberOfPages() ; i++)
            {
                var strategy = new LocationTextExtractionStrategy();
                var page = pdfDocument.GetPage(i);
                var text = PdfTextExtractor.GetTextFromPage(page, strategy);
                processed.Append(text);
                processed.Append("\n");
            }
            pdfDocument.Close();
            return Convert.ToString(processed);
        }
    }
}