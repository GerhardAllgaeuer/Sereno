using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Sereno.Test.Database
{
    public static class DataTableAssertionsExtensions
    {
        public static void ShouldBeEquivalentTo<T>(this DataTable actualTable, IEnumerable<T> expectedRows)
        {
            var actualRows = actualTable.AsEnumerable()
                .Select(row => actualTable.Columns.Cast<DataColumn>()
                    .ToDictionary(col => col.ColumnName, col => row[col] == DBNull.Value ? null : row[col]))
                .ToList();

            var expectedRowDicts = expectedRows
                .Select(obj => obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null)))
                .ToList();

            if (actualRows.Count != expectedRowDicts.Count)
            {
                throw new Exception($"Row count mismatch: actual {actualRows.Count}, expected {expectedRowDicts.Count}");
            }

            for (int i = 0; i < actualRows.Count; i++)
            {
                var actualRow = actualRows[i];
                var expectedRow = expectedRowDicts[i];

                // Check if both have same keys
                var actualKeys = actualRow.Keys.OrderBy(k => k).ToList();
                var expectedKeys = expectedRow.Keys.OrderBy(k => k).ToList();

                if (!actualKeys.SequenceEqual(expectedKeys, StringComparer.OrdinalIgnoreCase))
                {
                    throw new Exception($"Column mismatch in row {i + 1}: actual columns [{string.Join(", ", actualKeys)}], expected columns [{string.Join(", ", expectedKeys)}]");
                }

                // Compare values
                foreach (var key in actualKeys)
                {
                    var actualValue = actualRow[key];
                    var expectedValue = expectedRow[key];

                    if (!AreValuesEqual(actualValue, expectedValue))
                    {
                        throw new Exception($"Value mismatch in row {i + 1}, column '{key}': actual '{actualValue}', expected '{expectedValue}'");
                    }
                }
            }
        }


        private static bool AreValuesEqual(object actual, object expected)
        {
            if (actual == null && expected == null)
                return true;

            if (actual == null || expected == null)
                return false;

            // Try to convert both to string for comparison
            var actualStr = actual.ToString();
            var expectedStr = expected.ToString();

            return string.Equals(actualStr, expectedStr, StringComparison.Ordinal);
        }

    }
}
