namespace Sereno.Test.Database
{
    public class DatabaseRowsAssertion
    {
        private readonly List<Dictionary<string, object>> rows;

        public DatabaseRowsAssertion(List<Dictionary<string, object>> rows)
        {
            this.rows = rows;
        }

        public DatabaseRowsAssertionHelper Should()
        {
            return new DatabaseRowsAssertionHelper(rows);
        }
    }
}
