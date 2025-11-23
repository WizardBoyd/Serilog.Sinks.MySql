

namespace Serilog.Sinks.MySql.MySql.ColumnWriters
{
    public sealed class DefaultColumnWriter
    {
        public string Name { get; set; } = string.Empty;
        public int? Order { get; set; }
    }
}
