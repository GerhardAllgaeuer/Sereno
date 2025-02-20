using Sereno.Utilities;
using System.Data;
using System.Reflection;

namespace Sereno.Office.Tables
{

    /// <summary>
    /// Utility-Klasse für Tabellen
    /// </summary>
    public class TableUtility
    {



        /// <summary>
        /// Konvertieren einer Objektliste in ein DataSet
        /// </summary>
        public static DataTable? GetDataSetFromObjectList<T>(List<T> objectList, TableInfo tableInfo)
        {
            DataTable? result = null;

            if (tableInfo != null &&
                tableInfo.Columns != null &&
                objectList != null &&
                objectList.Count > 0)
            {
                result = new DataTable(tableInfo.TableName);

                // Erstelle ein DataSet und füge ihm eine Tabelle hinzu
                DataSet dataSet = new DataSet("DataSet");
                dataSet.Tables.Add(result);

                foreach (TableColumn column in tableInfo.Columns)
                {
                    // Ermittle den Propertytyp basierend auf dem Mapping

                    PropertyInfo? propertyInfo = ObjectUtility.GetPropertyInfo(objectList[0], column.SourceProperty);
                    if (propertyInfo != null)
                    {
                        if (propertyInfo.PropertyType.IsEnum)
                        {
                            result.Columns.Add(column.ColumnName, typeof(string));
                        }
                        else
                        {
                            result.Columns.Add(column.ColumnName, propertyInfo.PropertyType);
                        }
                    }
                    else
                    {
                        result.Columns.Add(column.ColumnName, typeof(string));
                    }
                }

                foreach (var obj in objectList)
                {
                    if (obj != null)
                    {
                        // Füge die Daten aus den Objekten in die Tabelle ein, basierend auf dem Mapping
                        var row = result.NewRow();
                        foreach (TableColumn column in tableInfo.Columns)
                        {
                            if (!string.IsNullOrWhiteSpace(column.ColumnName))
                            {
                                object? value = ObjectUtility.GetPropertyValue(obj, column.SourceProperty);
                                row[column.ColumnName] = value;
                            }
                        }
                        result.Rows.Add(row);
                    }
                }
            }
            return result;
        }


    }
}
