

using Serilog.Sinks.MySql.MySql.ColumnWriters;
using Serilog.Sinks.MySql.MySql.EventArgs;

namespace Serilog.Sinks.MySql.MySql
{
    public sealed class MySqlOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public IFormatProvider? FormatProvider { get; set; }

        public string TableName { get; set; } = string.Empty;

        public string SchemaName {  get; set; } = string.Empty;

        public bool UseBulkInsert { get; set; }

        public bool EmitBulkWarnings { get; set; }

        public IDictionary<string, ColumnWriterBase> ColumnOptions { get; set; } = new Dictionary<string, ColumnWriterBase>();

        public TimeSpan Period {  get; set; }

        public bool NeedAutoCreateTable { get; set; }

        public int BatchSizeLimit { get; set; }

        public int QueueLimit { get; set; }

        public Action<CreateTableEventArgs>? OnCreateTable {  get; set; }

        public TimeSpan? RetentionTime { get; set; }
    }
}
