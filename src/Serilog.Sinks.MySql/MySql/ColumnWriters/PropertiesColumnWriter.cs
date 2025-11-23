using MySqlConnector;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.Text;

namespace Serilog.Sinks.MySql.MySql.ColumnWriters
{
    public sealed class PropertiesColumnWriter : ColumnWriterBase
    {
        public PropertiesColumnWriter() : base(MySqlDbType.JSON, order: 0)
        {
        }

        public PropertiesColumnWriter(MySqlDbType dbType = MySqlDbType.JSON, int? order = null)
       : base(dbType, order: order)
        {
        }

        public override object GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
        {
            return PropertiesToJson(logEvent);
        }

        private static object PropertiesToJson(LogEvent logEvent)
        {
            if (logEvent.Properties.Count == 0)
            {
                return "{}";
            }

            var valuesFormatter = new JsonValueFormatter();

            var sb = new StringBuilder();

            sb.Append('{');

            using (var writer = new StringWriter(sb))
            {
                foreach (var keyValuePair in logEvent.Properties)
                {
                    sb.Append($"`{keyValuePair.Key}`:");
                    valuesFormatter.Format(keyValuePair.Value, writer);
                    sb.Append(", ");
                }
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append('}');

            return sb.ToString();
        }

    }
}
