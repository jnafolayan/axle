using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;

namespace Axle.Engine.FileParsers
{
    /// <summary>
    /// Parser for pptx files
    /// </summary>
    public class PptxFileParser : FileParserBase
    {
        /// <summary>
        /// Parses a pptx file and returns the contents as a string
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>The text contained in the file as a string</returns>
        public override string ParseLocalFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            int numberOfSlides = CountSlides(filePath);
            string slideText;
            StringBuilder content = new StringBuilder();
            for (int i = 0; i < numberOfSlides; i++)
            {
                GetSlideIdAndText(out slideText, filePath, i);
                content.Append(slideText);
            }
            return Convert.ToString(content);
        }

        public int CountSlides(string presentationFile)
        {
            // Open the presentation as read-only.
            using (PresentationDocument presentationDocument = PresentationDocument.Open(presentationFile, false))
            {
                // Pass the presentation to the next CountSlides method and return the slide count.
                return CountSlides(presentationDocument);
            }
        }

        // Count the slides in the presentation.
        public int CountSlides(PresentationDocument presentationDocument)
        {
            // Check for a null document object.
            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            int slidesCount = 0;

            // Get the presentation part of document.
            PresentationPart presentationPart = presentationDocument.PresentationPart;
            // Get the slide count from the SlideParts.
            if (presentationPart != null)
            {
                slidesCount = presentationPart.SlideParts.Count();
            }
            // Return the slide count to the previous method.
            return slidesCount;
        }

        public void GetSlideIdAndText(out string sldText, string docName, int index)
        {
            using (PresentationDocument ppt = PresentationDocument.Open(docName, false))
            {
                // Get the relationship ID of the first slide.
                PresentationPart part = ppt.PresentationPart;
                OpenXmlElementList slideIds = part.Presentation.SlideIdList.ChildElements;

                string relId = (slideIds[index] as SlideId).RelationshipId;

                // Get the slide part from the relationship ID.
                SlidePart slide = (SlidePart)part.GetPartById(relId);

                // Build a StringBuilder object.
                StringBuilder paragraphText = new StringBuilder();

                // Get the inner text of the slide:
                IEnumerable<DocumentFormat.OpenXml.Drawing.Text> texts = slide.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Text>();
                foreach (DocumentFormat.OpenXml.Drawing.Text text in texts)
                {
                    paragraphText.Append(text.Text);
                }
                sldText = paragraphText.ToString();
            }
        }
    }
}