

namespace Serilog.Sinks.MySql.IntegrationTests.Objects
{
    public sealed class TestObjectType2
    {
        public DateTime DateProp { get; set; }

        public TestObjectType1 NestedProp { get; set; } = new();
    }
}
