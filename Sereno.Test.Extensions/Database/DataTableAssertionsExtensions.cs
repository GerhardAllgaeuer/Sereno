using FluentAssertions;
using System.Data;

namespace Sereno.Test.Database
{
    public static class DataTableAssertionsExtensions
    {
        public static void ShouldBeEquivalentTo<T>(this DataTable actualTable, IEnumerable<T> expectedRows) where T : class
        {
            var actualRows = actualTable.AsEnumerable()
                .Select(row =>
                    Activator.CreateInstance(typeof(T),
                        row.ItemArray.Select(field => field?.ToString() ?? "").ToArray()) as T)
                .ToList();

            actualRows.Should().BeEquivalentTo(expectedRows, options => options.WithoutStrictOrdering());
        }
    }
}
