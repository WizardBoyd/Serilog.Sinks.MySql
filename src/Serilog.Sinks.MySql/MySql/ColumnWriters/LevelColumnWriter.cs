

using MySqlConnector;
using Serilog.Events;

namespace Serilog.Sinks.MySql.MySql.ColumnWriters
{
    public sealed class LevelColumnWriter : ColumnWriterBase
    {
        private readonly bool renderAsText;

        public LevelColumnWriter()
            : base(MySqlDbType.Int32, order: 0)
        {
            
        }

        public LevelColumnWriter(bool renderAsText = false, MySqlDbType dbType = MySqlDbType.Int32, int? order = null)
            : base(dbType, order: order)
        {
            this.renderAsText = renderAsText;
            if (this.renderAsText)
            {
                this.DbType = MySqlDbType.Text;
            }
        }

        public override object GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
        {
            if (this.renderAsText)
            {
                return logEvent.Level.ToString();
            }

            return (int)logEvent.Level;
        }
    }
}
