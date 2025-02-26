using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Sereno.Utilities.TableConverter
{

    /// <summary>
    /// Utility-Klasse für Tabellen
    /// </summary>
    public class TableConverterUtility
    {


        /// <summary>
        /// Konvertieren einer Objektliste in ein DataSet
        /// </summary>
        public static DataSet DataSetFromObjectList<T>(List<T> objectList, MappingInfo tableInfo)
        {
            DataSet result = new DataSet();
            DataTable table = DataTableFromObjectList<T>(objectList, tableInfo);

            return table.DataSet;
        }

        /// <summary>
        /// Konvertieren einer Objektliste in eine DataTable
        /// </summary>
        public static DataTable DataTableFromObjectList<T>(List<T> objectList, MappingInfo tableInfo)
        {
            DataTable result = null;

            if (tableInfo != null &&
                tableInfo.Columns != null &&
                objectList != null &&
                objectList.Count > 0)
            {
                result = new DataTable(tableInfo.TableName);

                // Erstelle ein DataSet und füge ihm eine Tabelle hinzu
                DataSet dataSet = new DataSet("DataSet");
                dataSet.Tables.Add(result);

                foreach (MappingColumn column in tableInfo.Columns)
                {
                    // Ermittle den Propertytyp basierend auf dem Mapping

                    PropertyInfo propertyInfo = ObjectUtility.GetPropertyInfo(objectList[0], column.SourceProperty);
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
                        foreach (MappingColumn column in tableInfo.Columns)
                        {
                            if (!string.IsNullOrWhiteSpace(column.ColumnName))
                            {
                                object value = ObjectUtility.GetPropertyValue(obj, column.SourceProperty);
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
