

using MySqlConnector;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.Text;

namespace Serilog.Sinks.MySql.MySql.ColumnWriters
{
    public sealed class LogEventSerializedColumnWriter : ColumnWriterBase
    {
        public LogEventSerializedColumnWriter()
            : base(MySqlDbType.JSON, order: 0)
        {
            
        }

        public LogEventSerializedColumnWriter(MySqlDbType dbType = MySqlDbType.JSON, int? order = null)
           : base(dbType, order: order)
        {

        }

        public override object GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
        {
            return LogEventToJson(logEvent, formatProvider);
        }

        private static object LogEventToJson(LogEvent logEvent, IFormatProvider? formatProvider)
        {
            var jsonFormatter = new JsonFormatter(formatProvider: formatProvider);

            var sb = new StringBuilder();
            using var writer = new StringWriter(sb);
            jsonFormatter.Format(logEvent, writer);

            return sb.ToString();
        }
    }
}
