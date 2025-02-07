using System.Collections.Generic;
using System.Data;
using Dapper;
using FluentAssertions;
using Microsoft.Data.SqlClient;

namespace Sereno.Test.Database
{
    public class DataRowAssertion
    {
        private readonly Dictionary<string, object> rowData;

        public DataRowAssertion(SqlConnection connection, string table, string primaryKeyColumn, object primaryKeyValue)
        {
            // Sicherstellen, dass die Verbindung offen ist
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            // SQL-Abfrage mit Parametern (SQL-Injection-Schutz)
            string query = $"SELECT * FROM [{table}] WHERE [{primaryKeyColumn}] = @PrimaryKeyValue";

            // Query ausführen
            var result = connection.QueryFirstOrDefault<dynamic>(query, new { PrimaryKeyValue = primaryKeyValue });

            // Falls keine Zeile gefunden wurde, ein leeres Dictionary setzen
            if (result == null)
            {
                rowData = new Dictionary<string, object>();
            }
            else
            {
                // ExpandoObject zu Dictionary umwandeln
                rowData = new Dictionary<string, object>((IDictionary<string, object>)result);
            }

            // Assertion: Zeile muss existieren
            rowData.Should().NotBeEmpty($"Erwartet wurde eine Zeile in '{table}' mit {primaryKeyColumn} = '{primaryKeyValue}', aber keine wurde gefunden.");
        }


        public ColumnAssertion Column(string columnName)
        {
            rowData.Should().ContainKey(columnName, $"Die Spalte '{columnName}' existiert nicht in der Zeile.");
            return new ColumnAssertion(rowData[columnName]);
        }
    }
}
