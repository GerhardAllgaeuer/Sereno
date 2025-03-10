using ClosedXML.Excel;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Sereno.Test.Excel
{
    public static class ExcelExtensions
    {
        public static ExcelTableAssertion Should(this IXLTable table)
        {
            return new ExcelTableAssertion(table);
        }

        public static ExcelTableAssertion Table(this XLWorkbook workbook, string sheetName = null, string tableName = null)
        {
            var sheet = string.IsNullOrWhiteSpace(sheetName)
                ? workbook.Worksheets.FirstOrDefault()
                : workbook.Worksheet(sheetName);

            if (sheet is null)
            {
                throw new AssertionFailedException($"Das Tabellenblatt '{sheetName}' existiert nicht.");
            }

            var table = string.IsNullOrWhiteSpace(tableName)
                ? sheet.Tables.FirstOrDefault()
                : sheet.Table(tableName);

            if (table is null)
            {
                throw new AssertionFailedException($"Die Tabelle '{tableName}' existiert nicht in '{sheet.Name}'.");
            }

            return new ExcelTableAssertion(table);
        }
    }

    public class ExcelTableAssertion
    {
        private readonly IXLTable _table;

        public ExcelTableAssertion(IXLTable table)
        {
            _table = table;
        }

        public void HaveRowCount(int expectedRowCount, string because = "", params object[] becauseArgs)
        {
            int actualRowCount = _table.DataRange.RowCount();

            using (new AssertionScope())
            {
                actualRowCount.Should().Be(expectedRowCount, because, becauseArgs);
            }
        }

        public void ContainsValues(IEnumerable<object> expectedRows, string because = "", params object[] becauseArgs)
        {
            var columnNames = _table.Fields.Select(f => f.Name).ToList();
            var tableRows = _table.DataRange.Rows()
                .Select(row => row.Cells().ToDictionary(cell => columnNames[cell.Address.ColumnNumber - 1], cell => cell.Value.ToString()))
                .ToList();

            var expectedRowDicts = expectedRows.Select(row => row.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(row)?.ToString() ?? ""))
                .ToList();

            using (new AssertionScope())
            {
                foreach (var expectedRow in expectedRowDicts)
                {
                    tableRows.Should().Contain(row => expectedRow.All(e => row.ContainsKey(e.Key) && row[e.Key] == e.Value), because, becauseArgs);
                }
            }
        }

        public ExcelColumnAssertion Column(string columnName)
        {
            var column = _table.DataRange.ColumnsUsed()
                .FirstOrDefault(col => _table.Fields.ElementAt(col.ColumnNumber() - 1).Name == columnName);

            if (column is null)
            {
                throw new AssertionFailedException($"Die Spalte '{columnName}' existiert nicht in der Tabelle '{_table.Name}'.");
            }

            return new ExcelColumnAssertion(column, columnName, _table.Name);
        }
    }

    public class ExcelColumnAssertion
    {
        private readonly IXLRangeColumn _column;
        private readonly string _columnName;
        private readonly string _tableName;

        public ExcelColumnAssertion(IXLRangeColumn column, string columnName, string tableName)
        {
            _column = column;
            _columnName = columnName;
            _tableName = tableName;
        }

        public void ContainsValues(IEnumerable<string> expectedValues, string because = "", params object[] becauseArgs)
        {
            var actualValues = _column.Cells().Select(cell => cell.GetString()).ToList();

            using (new AssertionScope())
            {
                foreach (var expectedValue in expectedValues)
                {
                    actualValues.Should().Contain(expectedValue, because, becauseArgs);
                }
            }
        }
    }
}