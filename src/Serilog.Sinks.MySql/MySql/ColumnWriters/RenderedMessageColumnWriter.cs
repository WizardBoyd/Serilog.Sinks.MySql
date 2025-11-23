using MySqlConnector;
using Serilog.Events;

namespace Serilog.Sinks.MySql.MySql.ColumnWriters
{
    public sealed class RenderedMessageColumnWriter : ColumnWriterBase
    {

        public RenderedMessageColumnWriter() : base(MySqlDbType.Text, order: 0)
        {
        }

        public RenderedMessageColumnWriter(MySqlDbType dbType = MySqlDbType.Text, int? order = null)
        : base(dbType, order: order)
        {
        }

        public override object GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
        {
            object value = logEvent.RenderMessage(formatProvider);
            return value;
        }

    }
}
