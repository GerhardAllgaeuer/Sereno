using FluentAssertions;

namespace Sereno.Test.Database
{
    public class DatabaseRowsAssertionHelper
    {
        private readonly List<Dictionary<string, object>> rows;

        public DatabaseRowsAssertionHelper(List<Dictionary<string, object>> rows)
        {
            this.rows = rows;
        }

        public void ContainValues(object[] expectedRows)
        {
            var expectedDicts = expectedRows.Select(obj =>
                obj.GetType().GetProperties()
                    .ToDictionary(p => p.Name, p => p.GetValue(obj))
            ).ToList();

            rows.Should().AllSatisfy(row =>
            {
                var filteredRow = row.Where(kvp => expectedDicts.Any(e => e.ContainsKey(kvp.Key)))
                                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                expectedDicts.Should().ContainEquivalentOf(filteredRow);
            });
        }
    }
}
