using MySqlConnector;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.Text;

namespace Serilog.Sinks.MySql.MySql.ColumnWriters
{
    public sealed class SinglePropertyColumnWriter : ColumnWriterBase
    {
        public string? Format { get; set; }
        public string Name { get; set; } = string.Empty;
        public PropertyWriteMethod WriteMethod { get; set; }

        public SinglePropertyColumnWriter() : base(MySqlDbType.Text, order: 0)
        {
        }

        public SinglePropertyColumnWriter(int? order = null) : base(MySqlDbType.Text, order: order)
        {
        }

        public SinglePropertyColumnWriter(
        string name,
        PropertyWriteMethod writeMethod = PropertyWriteMethod.ToString,
        MySqlDbType dbType = MySqlDbType.Text,
        string? format = null,
        int? order = null)
        : base(dbType, order: order)
        {
            this.Name = name;
            this.WriteMethod = writeMethod;
            this.Format = format;
        }

        public override object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
        {
            if (!logEvent.Properties.ContainsKey(this.Name))
            {
                return DBNull.Value;
            }

            switch (this.WriteMethod)
            {
                case PropertyWriteMethod.Raw:
                    return GetPropertyValue(logEvent.Properties[this.Name]);
                case PropertyWriteMethod.Json:
                    var valuesFormatter = new JsonValueFormatter();

                    var sb = new StringBuilder();

                    using (var writer = new StringWriter(sb))
                    {
                        valuesFormatter.Format(logEvent.Properties[this.Name], writer);
                    }

                    return sb.ToString();

                default:
                    return logEvent.Properties[this.Name].ToString(this.Format, formatProvider);
            }
        }

        private static object? GetPropertyValue(LogEventPropertyValue logEventProperty)
        {
            // TODO: Add support for arrays
            if (logEventProperty is ScalarValue scalarValue)
            {
                // In case of an enum and write method "raw", write it as integer.
                if (scalarValue.Value is Enum enumValue)
                {
                    return Convert.ToInt32(enumValue);
                }

                return scalarValue?.Value;
            }

            return logEventProperty;
        }
    }
}
