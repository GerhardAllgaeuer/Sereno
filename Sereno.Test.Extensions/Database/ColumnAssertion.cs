using FluentAssertions;

namespace Sereno.Test.Database
{
    public class ColumnAssertion
    {
        private readonly object actualValue;

        public ColumnAssertion(object value)
        {
            actualValue = value;
        }

        public FluentAssertions.Primitives.ObjectAssertions Should()
        {
            return actualValue.Should();
        }
    }
}
