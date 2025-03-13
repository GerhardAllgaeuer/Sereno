using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Data;
using System.Linq;

namespace Sereno.Office.Word.SimpleStructure
{
    public class TableGroupUtility
    {

        /// <summary>
        /// Tabellen Gruppen Objekt erstellen
        /// </summary>
        public static TableGroup ProcessTable(Table table)
        {
            TableGroup result = new TableGroup()
            {
                Table = table,
            };

            result.TableInfo = GetTableInfo(result);

            return result;
        }



        private static bool SetHeaderRowInfo(Table wordTable, TableInfoOptions options, TableInfo tableInfo)
        {
            bool result = false;

            if (options.DetermineHeaderRow)
            {
                TableLook look = wordTable.GetFirstChild<TableProperties>()?.GetFirstChild<TableLook>();
                if (look != null)
                {
                    if (look.FirstRow != null &&
                        look.FirstRow.HasValue &&
                        look.FirstRow.Value == true)
                    {
                        tableInfo.HasHeader = true;
                    }
                }
            }
            else
            {
                tableInfo.HasHeader = options.HasHeaderRow;
            }

            return result;
        }



        /// <summary>
        /// Daten als DataTable auslesen
        /// </summary>
        public static TableInfo GetTableInfo(TableGroup tableGroup, TableInfoOptions options = null)
        {
            if (options == null)
            {
                options = new TableInfoOptions();
            }

            TableInfo result = new TableInfo()
            {
                Data = new DataTable(),
            };


            DataTable dataTable = result.Data;
            Table wordTable = tableGroup.Table;


            SetHeaderRowInfo(wordTable, options, result);


            var rows = wordTable.Elements<TableRow>().ToList();
            var firstRowCells = rows[0].Elements<TableCell>().ToList();
            int columnCount = firstRowCells.Count;


            if (result.HasHeader)
            {
                foreach (var cell in firstRowCells)
                {
                    string columnName = GetCellText(cell);
                    dataTable.Columns.Add(columnName);
                    result.Columns.Add(new ColumnInfo()
                    {
                        ColumnName = columnName,
                        ColumnWidth = GetCellWidth(cell)
                    });
                }
                rows.RemoveAt(0);
            }
            else
            {
                for (int i = 0; i < columnCount; i++)
                {
                    string columnName = $"Column{i + 1}";
                    dataTable.Columns.Add(columnName);
                    result.Columns.Add(new ColumnInfo()
                    {
                        ColumnName = columnName,
                        ColumnWidth = GetCellWidth(firstRowCells[i])
                    });
                }
            }

            foreach (var row in rows)
            {
                var cells = row.Elements<TableCell>().ToList();
                DataRow dataRow = dataTable.NewRow();

                for (int i = 0; i < columnCount; i++)
                {
                    dataRow[i] = i < cells.Count ? GetCellText(cells[i]) : string.Empty;
                }

                dataTable.Rows.Add(dataRow);
            }

            return result;
        }

        private static string GetCellText(TableCell cell)
        {
            return cell.Descendants<Text>().Aggregate("", (current, text) => current + text.Text);
        }

        private static double? GetCellWidth(TableCell cell)
        {
            var cellProperties = cell.GetFirstChild<TableCellProperties>();
            if (cellProperties == null) return null;

            var cellWidth = cellProperties.GetFirstChild<TableCellWidth>();
            if (cellWidth == null || !cellWidth.Width.HasValue) return null;

            if (int.TryParse(cellWidth.Width.Value, out int twips))
            {
                return TwipsToCentimeters(twips);
            }

            return null;
        }

        private static double TwipsToCentimeters(int twips)
        {
            // 1 twip = 1/20 Punkt
            double points = twips / 20.0;

            // 1 Inch = 72 Punkte
            double inches = points / 72.0;
            
            // 1 cm = 2.54 Inch
            double result = inches * 2.54;

            result = Math.Round(result, 2);

            return result;
        }
    }
}
