using ClosedXML.Excel;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;

namespace Sereno.Test.Excel
{
    public static class ExcelExtensions
    {
        public static ExcelTableAssertion Table(this XLWorkbook workbook, string? sheetName = null, string? tableName = null)
        {
            var sheet = string.IsNullOrWhiteSpace(sheetName)
                ? workbook.Worksheets.FirstOrDefault()
                : workbook.Worksheet(sheetName);

            sheet.Should().NotBeNull($"Das Tabellenblatt '{sheetName}' existiert nicht.");

            var table = string.IsNullOrWhiteSpace(tableName)
                ? sheet.Tables.FirstOrDefault()
                : sheet.Table(tableName);

            table.Should().NotBeNull($"Die Tabelle '{tableName}' existiert nicht in '{sheet.Name}'.");

            return new ExcelTableAssertion(table!);
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
    }
}
