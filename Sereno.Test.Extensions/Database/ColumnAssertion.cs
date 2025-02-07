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

        public void Be(object expectedValue)
        {
            actualValue.Should().Be(expectedValue, $"Expected '{expectedValue}', was '{actualValue}'.");
        }
    }
}
