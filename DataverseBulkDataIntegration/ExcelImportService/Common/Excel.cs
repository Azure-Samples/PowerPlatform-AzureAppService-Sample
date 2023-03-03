// <copyright file="Excel.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Common
{
    using System.Reflection;
    using System.Text.RegularExpressions;
    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Spreadsheet;

    /// <summary>
    /// Operations related to parsing excel file.
    /// </summary>
    public static class Excel
    {
        /// <summary>
        /// Read from Excel file memory stream.
        /// </summary>
        /// <typeparam name="T">Object.</typeparam>
        /// <param name="memoryStream">Memory stream.</param>
        /// <returns>List of Objects.</returns>
        public static List<T> ReadExcelFromStream<T>(MemoryStream memoryStream)
            where T : new()
        {
            var excelRows = new List<T>();
            var excelRowsDict = new List<Dictionary<string, string?>>();
            var memoryStreamCopy = new MemoryStream(0);
            memoryStream.CopyTo(memoryStreamCopy);
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(memoryStreamCopy, true))
            {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                SharedStringTable sst = sstpart.SharedStringTable;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.Last();
                Worksheet sheet = worksheetPart.Worksheet;
                var cells = sheet.Descendants<Cell>();
                var rows = sheet.Descendants<Row>();

                int rowCounter = 0;
                var columnList = new List<string>();
                foreach (Row row in rows)
                {
                    bool isEmptyRow = false;
                    int cellCounter = 0;
                    var excelRow = new Dictionary<string, string?>();
                    foreach (Cell c in row.Elements<Cell>())
                    {
                        string? excelCellContent = null;
                        if ((c.DataType != null) && (c.DataType == CellValues.SharedString) && (c.CellValue != null))
                        {
                            int ssid = int.Parse(c.CellValue.Text);
                            string str = sst.ChildElements[ssid].InnerText;
                            if (rowCounter == 0)
                            {
                                // First row of excel containing column names
                                // Remove white spaces and underscore
                                var columnName = string.Concat(str.Where(c => !char.IsWhiteSpace(c))).Replace("_", string.Empty);
                                columnList.Add(columnName);
                            }
                            else
                            {
                                // Excel string cell values
                                excelCellContent = str;
                            }
                        }
                        else if (c.CellValue != null)
                        {
                            // Excel numeric cell values
                            excelCellContent = c.CellValue.Text;
                        }
                        else if ((cellCounter == 0) && (c.CellValue == null))
                        {
                            // Empty rows, that starts with empty cell contents
                            isEmptyRow = true;
                        }

                        // If it's detected as first cell in empty, do check if there are next cells that is having values, if yes reset the empty row.
                        if ((cellCounter != 0) && (c.CellValue != null) && isEmptyRow)
                        {
                            // Empty rows, that starts with empty cell contents
                            // TODO: This logic will fail if there are rows where the first cell in empty
                            isEmptyRow = false;
                        }

                        // Handle empty cells in a row
                        // TODO: This will break if there is a column in excel that is having empty value
                        int? cellColumnIndex = GetColumnIndexFromName(GetColumnName(c.CellReference));
                        cellColumnIndex--;
                        if (cellCounter < cellColumnIndex)
                        {
                            do
                            {
                                // Null values for empty cell
                                excelRow[columnList[cellCounter]] = null;
                                cellCounter++;
                            }
                            while (cellCounter < cellColumnIndex);
                        }

                        if (rowCounter != 0)
                        {
                            excelRow[columnList[cellCounter]] = excelCellContent;
                        }

                        cellCounter++;
                    }

                    if ((rowCounter != 0) && (isEmptyRow != true))
                    {
                        excelRowsDict.Add(excelRow);
                    }

                    rowCounter++;
                }
            }

            Console.WriteLine(excelRowsDict.Count());

            // Convert to object
            foreach (var row in excelRowsDict)
            {
                T rowObject = DictionaryToObject<T>(row);
                excelRows.Add(rowObject);
            }

            return excelRows;
        }

        /// <summary>
        /// Given just the column name (no row index), it will return the zero based column index.
        /// Note: This method will only handle columns with a length of up to two (ie. A to Z and AA to ZZ).
        /// A length of three can be implemented when needed.
        /// </summary>
        /// <param name="columnName">Column Name (ie. A or AB).</param>
        /// <returns>Zero based index if the conversion was successful; otherwise null.</returns>
        public static int? GetColumnIndexFromName(string columnName)
        {
            string name = columnName;
            int number = 0;
            int pow = 1;
            for (int i = name.Length - 1; i >= 0; i--)
            {
                number += (name[i] - 'A' + 1) * pow;
                pow *= 26;
            }

            return number;
        }

        /// <summary>
        /// Given a cell name, parses the specified cell to get the column name.
        /// </summary>
        /// <param name="cellReference">Address of the cell (ie. B2.)</param>
        /// <returns>Column Name (ie. B).</returns>
        public static string GetColumnName(string cellReference)
        {
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);
            return match.Value;
        }

        /// <summary>
        /// Convert dictionary to an object.
        /// </summary>
        /// <typeparam name="T">Object Type.</typeparam>
        /// <param name="dict">Input Dict.</param>
        /// <returns>Object.</returns>
        public static T DictionaryToObject<T>(IDictionary<string, string?> dict)
            where T : new()
        {
            var t = new T();
            PropertyInfo[] properties = t.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                KeyValuePair<string, string?> item = dict.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));

                // Find which property type (int, string, double? etc) the CURRENT property is...
                Type? tPropertyType = t.GetType().GetProperty(name: property.Name)?.PropertyType;

                // Fix nullables...
#pragma warning disable CS8604 // Possible null reference argument.
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;
#pragma warning restore CS8604 // Possible null reference argument.

                // ...and change the type
                object? newA = Convert.ChangeType(item.Value, newT);
                t.GetType().GetProperty(property.Name)?.SetValue(t, newA, null);
            }

            return t;
        }

        /// <summary>
        /// Create Excel Part.
        /// </summary>
        /// <param name="columNames">List of strings.</param>
        /// <param name="allDataRows">List os list of strings.</param>
        /// <returns>MemoryStream.</returns>
        public static MemoryStream GetExcelDocMemoryStream(List<string> columNames, List<List<string?>> allDataRows)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Auto Generated" };

                sheets.Append(sheet);

                workbookPart.Workbook.Save();

                SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                // Constructing header
                Row row = new Row();

                // Construcs Columns
                List<OpenXmlElement> columns = new();
                foreach (string c in columNames)
                {
                    columns.Add(ConstructCell(c, CellValues.String));
                }

                row.Append(columns.ToArray());

                // Insert the header row to the Sheet Data
                sheetData.AppendChild(row);

                // Inserting each employee
                foreach (List<string?> dataRow in allDataRows)
                {
                    row = new Row();

                    // Constructs data rows
                    List<OpenXmlElement> dataRows = new();
                    foreach (string? d in dataRow)
                    {
                        dataRows.Add(ConstructCell(d, CellValues.String));
                    }

                    row.Append(dataRows.ToArray());

                    sheetData.AppendChild(row);
                }

                worksheetPart.Worksheet.Save();
            }

            return memoryStream;
        }

        /// <summary>
        /// Construct cell.
        /// </summary>
        /// <param name="value">string.</param>
        /// <param name="dataType">CellValues.</param>
        /// <returns>Cell.</returns>
        public static Cell ConstructCell(string? value, CellValues dataType)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType),
            };
        }
    }
}
