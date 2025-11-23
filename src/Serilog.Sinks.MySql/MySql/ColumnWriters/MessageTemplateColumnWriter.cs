
using MySqlConnector;
using Serilog.Events;
namespace Serilog.Sinks.MySql.MySql.ColumnWriters
{
    public sealed class MessageTemplateColumnWriter : ColumnWriterBase
    {
        public MessageTemplateColumnWriter() : base(MySqlDbType.Text, order: 0)
        {
        }

        public MessageTemplateColumnWriter(MySqlDbType dbType = MySqlDbType.Text, int? order = null)
        : base(dbType, order: order)
        {
        }

        public override object GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
        {
            return logEvent.MessageTemplate.Text;
        }
    }
}
