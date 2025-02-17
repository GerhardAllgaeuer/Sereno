using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace Sereno.Database
{
    public class SchemaUtility
    {

        public static DataTable? GetDatabaseTables(string masterConnectionString, string databaseName)
        {
            string connectionString = ConnectionStringUtility.ChangeDatabaseName(masterConnectionString, databaseName);

            DataTable? mainTables = null;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                mainTables = connection.GetSchema("Tables");
            }

            FilterTables(mainTables);

            return mainTables;

        }

        private static void FilterTables(DataTable tables)
        {
            var rowsToDelete = tables.AsEnumerable()
                .Where(row =>
                    !string.IsNullOrEmpty(row.Field<string>("TABLE_NAME")) &&
                    row.Field<string>("TABLE_NAME")!.StartsWith("__EF"))
                .ToList();

            foreach (var row in rowsToDelete)
            {
                tables.Rows.Remove(row);
            }
        }


        public static DataTable GetTableColumns(string masterConnectionString, string databaseName, string tableName)
        {
            string connectionString = ConnectionStringUtility.ChangeDatabaseName(masterConnectionString, databaseName);

            DataTable? mainTables = null;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                mainTables = connection.GetSchema("Columns", [null, null, tableName, null]);
            }

            mainTables.Columns.Add("TYPE_FORMATTED", typeof(string));

            foreach (DataRow columnRow in mainTables.Rows)
            {
                AddColumnType(columnRow);
            }

            return mainTables;
        }


        private static void AddColumnType(DataRow columnRow)
        {
            string dataType = columnRow["DATA_TYPE"].ToString() ?? throw new InvalidOperationException("DATA_TYPE is missing.");
            int maxLength = columnRow["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value ? Convert.ToInt32(columnRow["CHARACTER_MAXIMUM_LENGTH"]) : -1;
            int precision = columnRow["NUMERIC_PRECISION"] != DBNull.Value ? Convert.ToInt32(columnRow["NUMERIC_PRECISION"]) : 0;
            int scale = columnRow["NUMERIC_SCALE"] != DBNull.Value ? Convert.ToInt32(columnRow["NUMERIC_SCALE"]) : 0;

            columnRow["TYPE_FORMATTED"] = GetColumnType(dataType, maxLength, precision, scale);
        }


        public static string GetColumnType(string dataType, int maxLength, int precision, int scale)
        {
            return dataType switch
            {
                "nvarchar" => maxLength > 0 ? $"nvarchar({maxLength})" : "nvarchar(max)",
                "varchar" => maxLength > 0 ? $"varchar({maxLength})" : "varchar(max)",
                "decimal" => $"decimal({precision}, {scale})",
                "numeric" => $"numeric({precision}, {scale})",
                "int" => "int",
                "bit" => "bit",
                "datetime" => "datetime",
                "datetime2" => "datetime2",
                "timestamp" => "timestamp",
                _ => throw new NotSupportedException($"Der Datentyp '{dataType}' wird nicht unterstützt."),
            };
        }

    }
}
