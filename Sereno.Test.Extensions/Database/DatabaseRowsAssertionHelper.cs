using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

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
            // Sicherstellen, dass beide Seiten nicht leer sind
            rows.Should().NotBeNull("tatsächliche Daten dürfen nicht null sein");
            rows.Should().NotBeEmpty("tatsächliche Daten dürfen nicht leer sein");

            expectedRows.Should().NotBeNull("erwartete Daten dürfen nicht null sein");
            expectedRows.Should().NotBeEmpty("erwartete Daten dürfen nicht leer sein");

            // Erwartete Objekte in Dictionaries umwandeln
            var expectedDicts = expectedRows.Select(obj =>
                obj.GetType().GetProperties()
                    .ToDictionary(p => p.Name, p => p.GetValue(obj))
            ).ToList();

            // Sicherstellen, dass die Anzahl exakt übereinstimmt
            rows.Count.Should().Be(expectedDicts.Count, "die Anzahl der Zeilen muss exakt übereinstimmen");

            // Nur die Spalten vergleichen, die in den erwarteten Objekten definiert sind
            var columnsToCompare = expectedDicts.First().Keys.ToArray();
            var filteredRows = rows.Select(row => 
                row.Where(kvp => columnsToCompare.Contains(kvp.Key))
                   .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            ).ToList();

            filteredRows.Should().BeEquivalentTo(expectedDicts);
        }

        public void ContainValues(List<object> expectedRows)
        {
            ContainValues(expectedRows.ToArray());
        }
    }
}
