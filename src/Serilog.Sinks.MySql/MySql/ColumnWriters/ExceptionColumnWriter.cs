

using MySqlConnector;
using Serilog.Events;

namespace Serilog.Sinks.MySql.MySql.ColumnWriters
{
    public sealed class ExceptionColumnWriter : ColumnWriterBase
    {
        public ExceptionColumnWriter()
            : base(MySqlDbType.Text, order: 0)
        {
            
        }

        public ExceptionColumnWriter(MySqlDbType dbType = MySqlDbType.Text, int? order = null)
            : base(dbType, order: order)
        {
            
        }

        public override object GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
        {
            return logEvent.Exception == null ? (object)DBNull.Value : logEvent.Exception.ToString();
        }
    }
}
