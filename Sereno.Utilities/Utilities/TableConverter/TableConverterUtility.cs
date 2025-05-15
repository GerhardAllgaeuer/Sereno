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
            DataTable table = DataTableFromObjectList<T>(objectList, tableInfo);
            return table.DataSet;
        }

        /// <summary>
        /// Konvertieren einer Objektliste in eine DataTable
        /// </summary>
        public static DataTable DataTableFromObjectList<T>(List<T> objectList, MappingInfo tableInfo)
        {
            if (tableInfo?.Columns == null || objectList == null || objectList.Count == 0)
            {
                return new DataTable();
            }

            var result = new DataTable(tableInfo.TableName);

            // Erstelle ein DataSet und füge ihm eine Tabelle hinzu
            var dataSet = new DataSet("DataSet");
            dataSet.Tables.Add(result);

            foreach (MappingColumn column in tableInfo.Columns)
            {
                if (string.IsNullOrWhiteSpace(column.SourceProperty))
                {
                    result.Columns.Add(column.ColumnName, typeof(string));
                    continue;
                }

                // Ermittle den Propertytyp basierend auf dem Mapping
                var propertyInfo = ObjectUtility.GetPropertyInfo(objectList[0], column.SourceProperty);
                if (propertyInfo != null)
                {
                    result.Columns.Add(column.ColumnName, 
                        propertyInfo.PropertyType.IsEnum ? typeof(string) : propertyInfo.PropertyType);
                }
                else
                {
                    result.Columns.Add(column.ColumnName, typeof(string));
                }
            }

            foreach (var obj in objectList)
            {
                if (obj == null) continue;

                // Füge die Daten aus den Objekten in die Tabelle ein, basierend auf dem Mapping
                var row = result.NewRow();
                foreach (MappingColumn column in tableInfo.Columns)
                {
                    if (!string.IsNullOrWhiteSpace(column.ColumnName))
                    {
                        object? value = ObjectUtility.GetPropertyValue(obj, column.SourceProperty);
                        row[column.ColumnName] = value ?? null;
                    }
                }
                result.Rows.Add(row);
            }

            return result;
        }


    }
}
