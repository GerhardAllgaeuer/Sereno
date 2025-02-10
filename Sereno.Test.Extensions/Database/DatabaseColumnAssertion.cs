using FluentAssertions;

namespace Sereno.Test.Database
{
    public class DatabaseColumnAssertion
    {
        private readonly object actualValue;

        public DatabaseColumnAssertion(object value)
        {
            actualValue = value;
        }

        public FluentAssertions.Primitives.ObjectAssertions Should()
        {
            return actualValue.Should();
        }
    }
}
