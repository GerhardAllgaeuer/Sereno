using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
