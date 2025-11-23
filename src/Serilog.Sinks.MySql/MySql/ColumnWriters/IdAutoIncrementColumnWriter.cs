

using MySqlConnector;
using Serilog.Events;

namespace Serilog.Sinks.MySql.MySql.ColumnWriters
{
    public sealed class IdAutoIncrementColumnWriter : ColumnWriterBase
    {
        public IdAutoIncrementColumnWriter()
            : base(MySqlDbType.Int64, true, 0)
        {
            
        }

        public IdAutoIncrementColumnWriter(int? order = null)
            : base(MySqlDbType.Int64, true, order: order)
        {

        }

        public override object GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
        {
            throw new Exception("Auto-increment column should not have a value to be written!");
        }

        public override string GetSqlType()
        {
            return $"{SqlTypeHelper.GetSqlTypeString(DbType)} AUTO_INCREMENT PRIMARY KEY";
        }

        public override Type GetNetType()
        {
            return typeof(long);
        }
    }
}
