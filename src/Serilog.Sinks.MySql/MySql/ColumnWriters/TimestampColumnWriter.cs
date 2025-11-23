using MySqlConnector;
using Serilog.Events;

namespace Serilog.Sinks.MySql.MySql.ColumnWriters
{
    public sealed class TimestampColumnWriter : ColumnWriterBase
    {
        public TimestampColumnWriter() : base(MySqlDbType.Timestamp, order: 0)
        {
        }

        public TimestampColumnWriter(MySqlDbType dbType = MySqlDbType.Timestamp, int? order = null)
       : base(dbType, order: order)
        {
           
            this.DbType = MySqlDbType.Timestamp;
        }

        public override object GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
        {
            //Had to add to UTC Date time to support bulk data insert with Sql Bulk Copy
            return logEvent.Timestamp.ToUniversalTime().UtcDateTime;
        }

    }
}
