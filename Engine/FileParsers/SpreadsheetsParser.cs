using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Axle.Engine.FileParsers
{
    /// <summary>
    /// Parser for spreadsheets (xls, xlsx)
    /// </summary>
    public class SpreadsheetsParser : FileParserBase
    {
        /// <summary>
        /// Parses a local file into text
        /// </summary>
        /// <param name="filepath">The path to the file</param>
        /// <returns>The text contained in the file</returns>
        public override string ParseLocalFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            var document = SpreadsheetDocument.Open(filePath, false);
            SharedStringTable sharedStringTable = document.WorkbookPart.SharedStringTablePart.SharedStringTable;
            string cellValue = null;
            StringBuilder content = new StringBuilder();

            foreach (WorksheetPart worksheetPart in document.WorkbookPart.WorksheetParts)
            {
                foreach (SheetData sheetData in worksheetPart.Worksheet.Elements<SheetData>())
                {
                    if (sheetData.HasChildren)
                    {
                        foreach (Row row in sheetData.Elements<Row>())
                        {
                            foreach (Cell cell in row.Elements<Cell>())
                            {
                                cellValue = cell.InnerText;
                                if (cell.DataType == CellValues.SharedString)
                                {
                                    content.Append(sharedStringTable.ChildElements[Int32.Parse(cellValue)].InnerText + " ");
                                }
                                else
                                {
                                    content.Append(cellValue + " ");
                                }
                            }
                        }
                    }
                }
            }

            return content.ToString();
        }
    }
}