using Microsoft.Data.SqlClient;
using Dapper;

namespace FluentAssertions.DapperExtensions
{
    public class DatabaseAssertions
    {
        private readonly SqlConnection connection;

        public DatabaseAssertions(SqlConnection connection)
        {
            this.connection = connection;
        }

        public DatabaseAssertions HaveTable(string tableName)
        {
            var result = connection.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName",
                new { TableName = tableName });

            result.Should().BeGreaterThan(0, $"Erwartet wurde, dass die Tabelle '{tableName}' existiert, aber sie wurde nicht gefunden.");

            return this;
        }


        public DatabaseAssertions HaveColumnType(string tableName, string columnName, string expectedType)
        {
            var actualType = connection.ExecuteScalar<string>(
                @"SELECT DATA_TYPE + 
                         CASE 
                             WHEN CHARACTER_MAXIMUM_LENGTH IS NOT NULL THEN '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
                             WHEN NUMERIC_PRECISION IS NOT NULL THEN '(' + CAST(NUMERIC_PRECISION AS VARCHAR) + ',' + CAST(NUMERIC_SCALE AS VARCHAR) + ')'
                             ELSE ''
                         END
                  FROM INFORMATION_SCHEMA.COLUMNS
                  WHERE TABLE_NAME = @TableName AND COLUMN_NAME = @ColumnName",
                new { TableName = tableName, ColumnName = columnName });

            actualType.Should().Be(expectedType,
                $"Erwartet: Spalte '{columnName}' in Tabelle '{tableName}' hat Typ '{expectedType}', aber war '{actualType}'.");

            return this;
        }
    }
}
