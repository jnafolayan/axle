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

            PdfReader pdfReader = new PdfReader(filePath);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            StringBuilder processed = new StringBuilder();
            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                string pageContent = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
                processed.Append(pageContent);
                processed.Append("\n");
            }
            pdfDoc.Close();
            pdfReader.Close();
            return Convert.ToString(processed);
        }
    }
}