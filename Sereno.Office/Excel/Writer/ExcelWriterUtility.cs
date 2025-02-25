using ClosedXML.Excel;
using Sereno.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sereno.Office.Excel.Writer
{
    public class ExcelWriterUtility
    {
        public static void InsertDataSetInExcel(string filePathName, DataSet dataSet, DataSetInsertOptions options)
        {
            using (var workbook = new XLWorkbook(filePathName))
            {

                var worksheet = workbook.Worksheet(1);
                System.Data.DataTable dataTable = dataSet.Tables[0];
                int startRowInExcel = 2;

                if (options != null)
                {
                    if (!String.IsNullOrEmpty(options.ExcelWorkSheetName))
                        worksheet = workbook.Worksheet(options.ExcelWorkSheetName);

                    if (!String.IsNullOrEmpty(options.DataSetTableName))
                        dataTable = dataSet.Tables[options.DataSetTableName];

                    startRowInExcel = options.StartRow;
                }


                // Spalten hinzufügen, meist für Pivot Tabellen
                CreateColumnsWithHeader(worksheet, dataTable, options);


                bool isInitialized = false;
                bool existingRows = false;
                TableInsertResult insertResult = new TableInsertResult();

                Dictionary<IXLCell, string> formulaDictionary = null;

                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        IXLRow rowToWrite = null;

                        existingRows = true;

                        // die erste Zeile wird überschrieben
                        if (!isInitialized)
                        {
                            rowToWrite = worksheet.Row(startRowInExcel);

                            insertResult.HeaderRow = worksheet.Row(startRowInExcel - 1);
                            insertResult.FirstRow = rowToWrite;
                            insertResult.FirstCell = rowToWrite.Cell(1);

                            isInitialized = true;
                        }
                        else
                        {
                            if (options == null ||
                                options.LineAdd)
                            {
                                rowToWrite = worksheet.Row(startRowInExcel - 1).InsertRowsBelow(1).First();
                            }
                            else
                            {
                                rowToWrite = worksheet.Row(startRowInExcel);
                            }

                            insertResult.LastRow = rowToWrite;
                        }

                        SetDataSetValues(dataRow, rowToWrite, options, insertResult);

                        if (formulaDictionary == null)
                            formulaDictionary = GetFormulaDictionaryFromFirstRow(worksheet, insertResult, rowToWrite);

                        if (formulaDictionary != null &&
                            formulaDictionary.Count > 0)
                            UpdateFormulasFromDictionary(rowToWrite, formulaDictionary);

                        startRowInExcel++;

                        if (options.StopAt.HasValue &&
                            startRowInExcel > options.StopAt.Value)
                        {
                            break;
                        }

                        // Debug.WriteLine(startRowInExcel);
                    }
                }
                else
                {
                    // In diesem Fall müssen wir die Beispiel Zeile im Template löschen
                    IXLRow rowToClear = worksheet.Row(startRowInExcel);
                    SetRowEmpty(rowToClear);

                }


                if (options.EmptyRowCountAfter > 0)
                {
                    for (int i = 0; i < options.EmptyRowCountAfter; i++)
                    {
                        IXLRow row = worksheet.Row(startRowInExcel - 1).InsertRowsBelow(1).First();

                        row.Style.Font.Bold = false;
                        row.Style.Fill.BackgroundColor = XLColor.NoColor;

                        row.Style.Border.SetBottomBorder(XLBorderStyleValues.None);
                        row.Style.Border.SetTopBorder(XLBorderStyleValues.None);
                        row.Style.Border.SetLeftBorder(XLBorderStyleValues.None);
                        row.Style.Border.SetRightBorder(XLBorderStyleValues.None);

                    }
                }


                if (options != null &&
                    options.CreateTable &&
                    existingRows)
                {
                    // Formatierung der Tabelle
                    IXLRange range = worksheet.Range(insertResult.HeaderRow.Cell(1), insertResult.LastCell);

                    // Mögliche Erweiterung der Spalten, wenn z.B. eine Summe am Ende der Tabelle steht
                    if (options.TableExpandColumns > 0)
                        range = ExpandRangeColumns(worksheet, range, options.TableExpandColumns);

                    var table = range.CreateTable();

                    table.FirstRow().Style.Fill.BackgroundColor = XLColor.FromHtml("#70AD47");

                    foreach (var dataRow in table.DataRange.RowsUsed())
                    {
                        foreach (var cell in dataRow.Cells())
                        {
                            cell.Style.Fill.BackgroundColor = XLColor.Transparent;
                        }
                    }

                    // AutoFilter hinzufügen
                    table.ShowAutoFilter = true;
                }



                workbook.SaveAs(filePathName);
            }
        }

        private static IXLRange ExpandRangeColumns(IXLWorksheet worksheet, IXLRange existingRange, int columns)
        {
            // Ermitteln des aktuellen Bereichsadresses
            var rangeAddress = existingRange.RangeAddress;

            // Erweitern des Bereichs um eine Spalte
            var newLastColumnNumber = rangeAddress.LastAddress.ColumnNumber + columns;

            // Sicherstellen, dass wir nicht über die maximale Spaltenzahl hinausgehen
            if (newLastColumnNumber <= worksheet.ColumnCount())
            {
                // Umwandlung der neuen Spaltennummer in den Buchstaben
                var newLastColumnName = XLHelper.GetColumnLetterFromNumber(newLastColumnNumber);

                // Erstellen des neuen Bereichsadresses als String
                var newRangeAddressString = $"{rangeAddress.FirstAddress}:{newLastColumnName}{rangeAddress.LastAddress.RowNumber}";

                // Erstellen des neuen Bereichs basierend auf dem neuen Adressstring
                return worksheet.Range(newRangeAddressString);
            }
            else
            {
                // Optional: Fehlerbehandlung oder einfach den bestehenden Bereich zurückgeben, falls die neue Spaltennummer ungültig ist
                // Hier entscheiden wir uns, den unveränderten Bereich zurückzugeben
                return existingRange;
            }
        }


        /// <summary>
        /// Formel von der ersten Zeile übernehmen
        /// </summary>
        static void UpdateFormulasFromDictionary(IXLRow rowToWrite, Dictionary<IXLCell, string> formulaDictionary)
        {
            foreach (KeyValuePair<IXLCell, string> formula in formulaDictionary)
            {
                string updatedFormula = UpdateFormulaRowReferences(formula.Value, rowToWrite.RowNumber());

                IXLCell cellToWrite = rowToWrite.Cell(formula.Key.Address.ColumnNumber);
                cellToWrite.FormulaA1 = updatedFormula;

            }
        }


        private static string UpdateFormulaRowReferences(string formula, int newRow)
        {
            return Regex.Replace(formula, @"([A-Z]+)(\d+)(?=\D|$)", m =>
            {
                // Prüfen, ob die gefundene Zahl eine Zeilennummer ist
                if (int.TryParse(m.Groups[2].Value, out int rowNum))
                {
                    // Ersetze die alte Zeilennummer durch die neue
                    return m.Groups[1].Value + newRow.ToString();
                }
                return m.Value;
            });
        }

        private static void SetRowEmpty(IXLRow row)
        {
            foreach (var cell in row.CellsUsed())
            {
                cell.SetValue(String.Empty);
            }
        }

        /// <summary>
        /// Formel von der ersten Zeile übernehmen
        /// </summary>
        private static Dictionary<IXLCell, string> GetFormulaDictionaryFromFirstRow(IXLWorksheet worksheet, TableInsertResult insertResult, IXLRow rowToWrite)
        {
            Dictionary<IXLCell, string> result = new Dictionary<IXLCell, string>();

            if (insertResult.FirstRow != null)
            {
                for (int col = 1; col <= worksheet.ColumnsUsed().Count(); col++)
                {
                    var cellAtFirstRow = insertResult.FirstRow.Cell(col);
                    if (cellAtFirstRow.HasFormula)
                    {
                        result.Add(cellAtFirstRow, cellAtFirstRow.FormulaA1);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Spalten automatisch hinzufügen, wenn diesem im Excel noch nicht vorhanden sind
        /// </summary>
        static void CreateColumnsWithHeader(IXLWorksheet worksheet, System.Data.DataTable dataTable, DataSetInsertOptions options)
        {
            // Spalten dynamisch hinzufügen (meistens für Pivot)
            if (options != null &&
                options.CreateColumnsWithHeader)
            {

                // Ggf. Summenformel hinter der Tabelle anpassen
                IXLCell sumCell = worksheet.Cell(options.StartRow, options.CreateColumnsStart + 1);
                string sumStartRange = "";

                // Überprüfen, ob in der Zelle eine SUM-Funktion enthalten ist
                if (sumCell.HasFormula && sumCell.FormulaA1.ToUpper().StartsWith("SUM"))
                {
                    // Extrahiere den aktuellen Bereich der Summenformel
                    string oldFormula = sumCell.FormulaA1;
                    sumStartRange = oldFormula.Substring(oldFormula.IndexOf('(') + 1, oldFormula.IndexOf(')') - oldFormula.IndexOf('(') - 1);
                }


                // Es gibt eine Start-Spalte im DataSet (CreateColumnsStart)
                // Ab dieser Spalte werden die folgenden Spalten im Excel automatisch hinzugefügt
                int countOfColumnsToAdd = dataTable.Columns.Count - options.CreateColumnsStart;

                if (countOfColumnsToAdd > 0)
                {

                    worksheet.Column(options.CreateColumnsStart + 1).InsertColumnsBefore(countOfColumnsToAdd);


                    // Ggf. Summe unter den hinzugefügten Spalten einfügen
                    ExtendSumsToAddedColumns(worksheet, options, countOfColumnsToAdd);


                    // Spalten Überschriften aus dem DataSet übernehmen
                    int startRow = options.StartRow - 1;
                    int startColumn = options.CreateColumnsStart;

                    // Durch die Spalten des DataSets iterieren und sie ins Excel-Dokument schreiben
                    for (int i = startColumn; i < dataTable.Columns.Count + 1; i++)
                    {
                        // Spaltennamen ab Spalte 3 und Zeile 4 einfügen
                        string columnName = dataTable.Columns[i - 1].ColumnName;
                        worksheet.Cell(startRow, i).Value = columnName;
                    }



                    // Ggf. Summenformel hinter der Tabelle anpassen
                    if (!String.IsNullOrEmpty(sumStartRange))
                    {
                        string newEndColumn = worksheet.Column(worksheet.Column(dataTable.Columns.Count).ColumnNumber()).ColumnLetter();
                        string newRange = $"{sumStartRange}:{newEndColumn}{options.StartRow}";

                        // Setze die neue Formel mit dem erweiterten Bereich
                        sumCell = worksheet.Cell(options.StartRow, dataTable.Columns.Count + 1);
                        sumCell.FormulaA1 = $"SUM({newRange})";
                    }
                }
            }
        }


        private static void ExtendSumsToAddedColumns(IXLWorksheet worksheet, DataSetInsertOptions options, int columnsAdded)
        {
            for (int rowCount = 0; rowCount < 4; rowCount++)
            {
                var cell = worksheet.Cell(options.StartRow + rowCount, options.CreateColumnsStart);

                if (cell.HasFormula && cell.FormulaA1.Contains("SUM"))
                {
                    for (int columnCount = 0; columnCount < columnsAdded; columnCount++)
                    {
                        int addColumnIndex = options.CreateColumnsStart + 1 + columnCount;
                        var neueZelle = worksheet.Cell(options.StartRow + rowCount, addColumnIndex);

                        // Bereich für die Summenformel festlegen (z.B. von Zeile 1 bis 9 in der jeweiligen Spalte)
                        string letterToReplace = XLHelper.GetColumnLetterFromNumber(options.CreateColumnsStart);
                        string sumRange = cell.FormulaA1.Replace(letterToReplace, XLHelper.GetColumnLetterFromNumber(addColumnIndex));

                        // Summenformel einfügen
                        neueZelle.FormulaA1 = $"=SUM({sumRange})";
                    }
                }
            }
        }


        private static void SetDataSetValues(DataRow dataRow, IXLRow excelRow, DataSetInsertOptions options, TableInsertResult insertResult)
        {
            int count = 1;

            foreach (var value in dataRow.ItemArray)
            {
                if (value != null)
                {

                    if (value.ToString() == "0")
                    {
                    }

                    Type t = value.GetType();

                    // Nur bei Tabellen werden die Style geändert
                    excelRow.Cell(count).Style.Font.Underline = XLFontUnderlineValues.None;
                    excelRow.Cell(count).Style.Font.FontColor = XLColor.Black;

                    if (value.GetType() == typeof(DateTime))
                    {
                        DateTime dt = (DateTime)value;
                        excelRow.Cell(count).Value = dt;
                    }
                    else if (value.GetType() == typeof(Decimal))
                    {
                        Decimal d = (Decimal)value;

                        if (options.ZeroAsEmpty &&
                            d == 0)
                        {
                            excelRow.Cell(count).Value = "";
                        }
                        else
                        {
                            excelRow.Cell(count).Value = d;
                        }

                    }
                    else if (value.GetType() == typeof(int))
                    {
                        int i = (int)value;

                        if (options.ZeroAsEmpty &&
                            i == 0)
                        {
                            excelRow.Cell(count).Value = "";
                        }
                        else
                        {
                            excelRow.Cell(count).Value = i;
                        }

                    }
                    else if (value.GetType() == typeof(System.DBNull))
                    {
                        if (excelRow.Cell(count).HasFormula)
                        {
                        }
                        else
                        {
                            excelRow.Cell(count).Value = Blank.Value;
                        }
                    }
                    else
                    {
                        if (options != null &&
                            options.Anonymize &&
                            options.AnonymizeColumns.Contains(dataRow.Table.Columns[count - 1].ColumnName))
                        {
                            excelRow.Cell(count).Value = StringUtility.AnonymizeString(value.ToString());
                        }
                        else if (value.ToString().StartsWith("URL"))
                        {
                            string urlValue = value.ToString();
                            urlValue = urlValue.Replace("URL:", "");
                            List<string> parts = urlValue.Split(new char[] { '|' }).ToList();
                            excelRow.Cell(count).Value = parts[0].ToString();
                            XLHyperlink link = excelRow.Cell(count).CreateHyperlink();
                            link.ExternalAddress = new Uri(parts[1]);
                            excelRow.Cell(count).SetHyperlink(link);
                            excelRow.Cell(count).Style.Font.FontColor = XLColor.FromHtml("#70AD47");
                            excelRow.Cell(count).Style.Font.Underline = XLFontUnderlineValues.Single;
                        }
                        else
                        {
                            string valueToSet = value.ToString();

                            if (valueToSet == "")
                            {
                                // Hier müssen wir explizit keinen Wert setzen, sonst gibt es in der Excel Datei einen Fehler
                                excelRow.Cell(count).Value = Blank.Value;
                            }
                            else
                            {
                                excelRow.Cell(count).Value = valueToSet;
                            }

                        }

                    }
                }
                else
                {
                    excelRow.Cell(count).Value = " ";
                    excelRow.Cell(count).Value = "";
                }


                // Delegate aufrufen
                IXLCell cell = excelRow.Cell(count);

                insertResult.LastCell = cell;

                if (options?.AfterSetCell != null)
                    options.AfterSetCell(cell, options);


                count++;
            }

            if (options?.AfterSetRow != null)
                options.AfterSetRow(excelRow, options);
        }





        /// <summary>
        /// Funktion, die nach dem Setzen eines Werte aufgerufen wird
        /// </summary>
        public delegate void AfterSetCellDelegate(IXLCell cell, DataSetInsertOptions options);

        /// <summary>
        /// Funktion, die nach dem Setzen eines Werte aufgerufen wird
        /// </summary>
        public delegate void AfterSetRowDelegate(IXLRow row, DataSetInsertOptions options);

        /// <summary>
        /// Funktion, die nach dem Setzen eines Werte aufgerufen wird
        /// </summary>
        public delegate void FormatRowDelegate(IXLRow row, DataSetInsertOptions options);

    }
}
