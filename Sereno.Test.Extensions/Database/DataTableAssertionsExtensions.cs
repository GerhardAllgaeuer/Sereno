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
            var actualRows = ConvertDataTableToRows(actualTable);
            var expectedRowDicts = ConvertObjectsToRows(expectedRows);

            if (actualRows.Count != expectedRowDicts.Count)
            {
                throw new Exception($"Row count mismatch: actual {actualRows.Count}, expected {expectedRowDicts.Count}");
            }

            CompareRows(actualRows, expectedRowDicts);
        }

        public static void ShouldContain<T>(this DataTable actualTable, IEnumerable<T> expectedRows)
        {
            var actualRows = ConvertDataTableToRows(actualTable);
            var expectedRowDicts = ConvertObjectsToRows(expectedRows);

            foreach (var expectedRow in expectedRowDicts)
            {
                if (!actualRows.Any(actualRow => AreRowsEqual(actualRow, expectedRow, compareAllColumns: false)))
                {
                    throw new Exception($"Expected row not found: [{string.Join(", ", expectedRow.Select(kvp => $"{kvp.Key}='{kvp.Value}'"))}]");
                }
            }
        }

        private static List<Dictionary<string, object>> ConvertDataTableToRows(DataTable table)
        {
            return table.AsEnumerable()
                .Select(row => table.Columns.Cast<DataColumn>()
                    .ToDictionary(col => col.ColumnName, col => row[col] == DBNull.Value ? null : row[col]))
                .ToList();
        }

        private static List<Dictionary<string, object>> ConvertObjectsToRows<T>(IEnumerable<T> objects)
        {
            return objects
                .Select(obj => obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null)))
                .ToList();
        }

        private static void CompareRows(List<Dictionary<string, object>> actualRows, List<Dictionary<string, object>> expectedRows)
        {
            for (int i = 0; i < actualRows.Count; i++)
            {
                if (!AreRowsEqual(actualRows[i], expectedRows[i]))
                {
                    throw new Exception($"Row mismatch at index {i}");
                }
            }
        }

        private static bool AreRowsEqual(Dictionary<string, object> actualRow, Dictionary<string, object> expectedRow, bool compareAllColumns = true)
        {
            if (compareAllColumns)
            {
                var actualKeys = actualRow.Keys.OrderBy(k => k).ToList();
                var expectedKeys = expectedRow.Keys.OrderBy(k => k).ToList();

                if (!actualKeys.SequenceEqual(expectedKeys, StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }

                return actualKeys.All(key => AreValuesEqual(actualRow[key], expectedRow[key]));
            }
            else
            {
                // Nur die Spalten in expectedRow vergleichen
                foreach (var expectedKey in expectedRow.Keys)
                {
                    if (!actualRow.ContainsKey(expectedKey))
                    {
                        return false;
                    }

                    if (!AreValuesEqual(actualRow[expectedKey], expectedRow[expectedKey]))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private static bool AreValuesEqual(object actual, object expected)
        {
            if (actual == null && expected == null)
                return true;

            if (actual == null || expected == null)
                return false;

            // Wenn beide Werte numerisch sind, konvertiere zu decimal für den Vergleich
            if (IsNumeric(actual) && IsNumeric(expected))
            {
                var actualDecimal = Convert.ToDecimal(actual);
                var expectedDecimal = Convert.ToDecimal(expected);

                actualDecimal = Math.Round(actualDecimal, 4);
                expectedDecimal = Math.Round(expectedDecimal, 4);

                return actualDecimal == expectedDecimal;
            }

            // Fallback auf String-Vergleich für nicht-numerische Werte
            var actualStr = actual.ToString();
            var expectedStr = expected.ToString();
            return string.Equals(actualStr, expectedStr, StringComparison.Ordinal);
        }

        private static bool IsNumeric(object value)
        {
            return value is sbyte || value is byte ||
                   value is short || value is ushort ||
                   value is int || value is uint ||
                   value is long || value is ulong ||
                   value is float || value is double ||
                   value is decimal;
        }
    }
}