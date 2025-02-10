using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

namespace Sereno.Test.Database
{
    public class DataRowsAssertion
    {
        private readonly List<Dictionary<string, object>> rows;

        public DataRowsAssertion(List<Dictionary<string, object>> rows)
        {
            this.rows = rows;
        }

        public DataRowsAssertionsWrapper Should()
        {
            return new DataRowsAssertionsWrapper(rows);
        }
    }
}
