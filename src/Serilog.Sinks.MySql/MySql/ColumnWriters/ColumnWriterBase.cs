

using MySqlConnector;
using Serilog.Events;

namespace Serilog.Sinks.MySql.MySql.ColumnWriters
{
    public abstract class ColumnWriterBase
    { 
        public MySqlDbType DbType { get; set; }

        public bool SkipOnInsert { get; set; }

        public int? Order {  get; set; }

        protected ColumnWriterBase(MySqlDbType dbType, bool skipOnInsert = false, int? order = null)
        {
            DbType = dbType;
            SkipOnInsert = skipOnInsert;
            Order = order;
        }

        /// <summary>
        /// Gets part of log event to write to the column
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public abstract object? GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null);

        public virtual string GetSqlType()
        {
            return SqlTypeHelper.GetSqlTypeString(DbType);
        }

        public virtual Type GetNetType()
        {
            return SqlTypeHelper.GetTypeForDbType(DbType);
        }
    }
}
