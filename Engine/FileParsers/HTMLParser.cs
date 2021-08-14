using System;
using System.IO;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace Axle.Engine.FileParsers
{
    /// <summary>
    /// Parser for html
    /// </summary>
    public class HTMLParser : FileParserBase
    {
        private List<string> ignoredElements = new List<string> { "script", "style" };
        private List<string> blockElements = new List<string> { "p", "div" };

        /// <summary>
        /// Parses an html into text
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>The text contained in the file</returns>
        public override string ParseLocalFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            HtmlDocument document = new HtmlDocument();
            document.Load(filePath);

            HtmlNode bodyNode = document.DocumentNode.SelectSingleNode("//body");
            string textContent = bodyNode != null ? bodyNode.InnerText : "";
            return textContent.Trim();
        }

        /// <summary>
        /// Parses an html file into a dictionary containing its title, description
        /// and body (content).
        /// </summary>
        /// <param name="filePath">The path to the html file</param>
        /// <returns>A dictionary</returns>
        public PageInformation ExtractHTMLFileInfo(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            StreamReader reader = File.OpenText(filePath);
            return ExtractHTMLStringInfo(reader.ReadToEnd());
        }

        /// <summary>
        /// Parses an html string into a dictionary containing its title, description
        /// and body (content).
        /// </summary>
        /// <param name="html">The html string</param>
        /// <returns>A dictionary</returns>
        public PageInformation ExtractHTMLStringInfo(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            HtmlNode titleNode = document.DocumentNode.SelectSingleNode("//head/title");
            HtmlNode descNode = document.DocumentNode.SelectSingleNode("//head/meta[name='description']");
            HtmlNode bodyNode = document.DocumentNode.SelectSingleNode("//body");

            string title = titleNode != null ? titleNode.InnerText : "";
            string description = descNode != null ? descNode.InnerText : "";
            string content = bodyNode != null ? bodyNode.InnerText : "";

            return new HTMLParser.PageInformation
            {
                Title = title.Trim(),
                Description = description.Trim(),
                Content = content.Trim()
            };
        }

        public class PageInformation
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Content { get; set; }
        }
    }
}