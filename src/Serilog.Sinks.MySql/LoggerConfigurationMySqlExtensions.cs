using Microsoft.Extensions.Configuration;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MySql.MySql;
using Serilog.Sinks.MySql.MySql.ColumnWriters;
using Serilog.Sinks.MySql.MySql.Configuration;
using Serilog.Sinks.MySql.MySql.EventArgs;
using MySqlConnector;

namespace Serilog
{
    public static class LoggerConfigurationMySqlExtensions
    {
        private const int DefaultBatchSizeLimit = 30;

        private const int DefaultQueueLimit = int.MaxValue;

        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(5);

        private static readonly IMicrosoftExtensionsConnectionStringProvider MicrosoftExtensionsConnectionStringProvider
        = new MicrosoftExtensionsConnectionStringProvider();


        public static LoggerConfiguration MySql(
            this LoggerSinkConfiguration sinkConfiguration,
            string connectionString,
            string tableName,
            IDictionary<string, ColumnWriterBase>? columnOptions = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            TimeSpan? period = null,
            IFormatProvider? formatProvider = null,
            int batchSizeLimit = DefaultBatchSizeLimit,
            int queueLimit = DefaultQueueLimit,
            LoggingLevelSwitch? levelSwitch = null,
            bool useBulkInsert = true,
            bool needAutoCreateTable = false,
            IConfiguration? appConfiguration = null,
            Action<CreateTableEventArgs>? onCreateTableCallback = null,
            TimeSpan? retentionTime = null)
            {
                if(sinkConfiguration is null)
                {
                    throw new ArgumentNullException(nameof(sinkConfiguration));
                }

                if(appConfiguration is not null)
                {
                    connectionString =
                        MicrosoftExtensionsConnectionStringProvider.GetConnectionString(connectionString, appConfiguration);
                }

                period ??= DefaultPeriod;

                var optionsLocal = GetOptions(
                    connectionString,
                    tableName,
                    columnOptions,
                    period.Value,
                    formatProvider,
                    batchSizeLimit,
                    queueLimit,
                    useBulkInsert,
                    needAutoCreateTable,
                    onCreateTableCallback,
                    retentionTime);

                var batchingOptions = new BatchingOptions()
                {
                    BatchSizeLimit = optionsLocal.BatchSizeLimit,
                    BufferingTimeLimit = optionsLocal.Period,
                    QueueLimit = optionsLocal.QueueLimit
                };
                return sinkConfiguration.Sink(new MySqlSink(optionsLocal), batchingOptions, restrictedToMinimumLevel, levelSwitch);
            }


        public static LoggerConfiguration MySql(
            this LoggerSinkConfiguration sinkConfiguration,
            string connectionString,
            string tableName,
            IDictionary<string, DefaultColumnWriter>? loggerColumnOptions = null,
            IDictionary<string, SinglePropertyColumnWriter>? loggerPropertyColumnOptions = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            TimeSpan? period = null,
            IFormatProvider? formatProvider = null,
            int batchSizeLimit = DefaultBatchSizeLimit,
            int queueLimit = DefaultQueueLimit,
            LoggingLevelSwitch? levelSwitch = null,
            bool useBulkInsert = true,
            bool needAutoCreateTable = false,
            IConfiguration? appConfiguration = null,
            Action<CreateTableEventArgs>? onCreateTableCallback = null,
            TimeSpan? retentionTime = null)
        {
            if (sinkConfiguration is null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            if (appConfiguration is not null)
            {
                connectionString =
                    MicrosoftExtensionsConnectionStringProvider.GetConnectionString(connectionString, appConfiguration);
            }

            period ??= DefaultPeriod;

            IDictionary<string, ColumnWriterBase>? columns = null;

            if (loggerColumnOptions is not null)
            {
                columns = new Dictionary<string, ColumnWriterBase>();

                foreach (var columnOption in loggerColumnOptions)
                {
                    var name = columnOption.Value.Name;

                    switch (name)
                    {
                        case "Level":
                            columns.Add(columnOption.Key, new LevelColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "LevelAsText":
                            columns.Add(columnOption.Key, new LevelColumnWriter(true, MySqlDbType.Text, columnOption.Value.Order));
                            break;
                        case "Timestamp":
                            columns.Add(columnOption.Key, new TimestampColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "LogEvent":
                            columns.Add(columnOption.Key, new LogEventSerializedColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "Properties":
                            columns.Add(columnOption.Key, new PropertiesColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "Message":
                            columns.Add(columnOption.Key, new MessageTemplateColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "RenderedMessage":
                            columns.Add(columnOption.Key, new RenderedMessageColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "Exception":
                            columns.Add(columnOption.Key, new ExceptionColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "IdAutoIncrement":
                            columns.Add(columnOption.Key, new IdAutoIncrementColumnWriter(order: columnOption.Value.Order));
                            break;
                    }
                }
            }

            if (loggerPropertyColumnOptions is null)
            {
                var optionsLocal = GetOptions(
                    connectionString,
                    tableName,
                    columns,
                    period.Value,
                    formatProvider,
                    batchSizeLimit,
                    queueLimit,
                    useBulkInsert,
                    needAutoCreateTable,
                    onCreateTableCallback,
                    retentionTime);

                var batchingOptions = new BatchingOptions()
                {
                    BatchSizeLimit = optionsLocal.BatchSizeLimit,
                    BufferingTimeLimit = optionsLocal.Period,
                    QueueLimit = optionsLocal.QueueLimit
                };

                return sinkConfiguration.Sink(new MySqlSink(optionsLocal), batchingOptions, restrictedToMinimumLevel, levelSwitch);
            }

            columns ??= new Dictionary<string, ColumnWriterBase>();

            foreach (var columnOption in loggerPropertyColumnOptions)
            {
                columns.Add(columnOption.Key, columnOption.Value);
            }

            var optionsLocal2 = GetOptions(
                       connectionString,
                       tableName,
                       columns,
                       period.Value,
                       formatProvider,
                       batchSizeLimit,
                       queueLimit,
                       useBulkInsert,
                       needAutoCreateTable,
                       onCreateTableCallback);

            var batchingOptions2 = new BatchingOptions()
            {
                BatchSizeLimit = optionsLocal2.BatchSizeLimit,
                BufferingTimeLimit = optionsLocal2.Period,
                QueueLimit = optionsLocal2.QueueLimit
            };

            return sinkConfiguration.Sink(new MySqlSink(optionsLocal2), batchingOptions2, restrictedToMinimumLevel, levelSwitch);
        }

        public static LoggerConfiguration MySql(
            this LoggerAuditSinkConfiguration sinkConfiguration,
            string connectionString,
            string tableName,
            IDictionary<string, ColumnWriterBase>? columnOptions = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider? formatProvider = null,
            LoggingLevelSwitch? levelSwitch = null,
            bool needAutoCreateTable = false,
            IConfiguration? appConfiguration = null,
            Action<CreateTableEventArgs>? onCreateTableCallback = null,
            TimeSpan? retentionTime = null)
        {
            if (sinkConfiguration is null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            if (appConfiguration is not null)
            {
                connectionString =
                    MicrosoftExtensionsConnectionStringProvider.GetConnectionString(connectionString, appConfiguration);
            }

            var optionsLocal = GetOptions(
            connectionString,
            tableName,
            columnOptions,
            TimeSpan.Zero,
            formatProvider,
            DefaultBatchSizeLimit,
            DefaultQueueLimit,
            false,
            needAutoCreateTable,
            onCreateTableCallback,
            retentionTime);

            return sinkConfiguration.Sink(new MySqlAuditSink(optionsLocal), restrictedToMinimumLevel, levelSwitch);
        }

        public static LoggerConfiguration MySql(
            this LoggerAuditSinkConfiguration sinkConfiguration,
            string connectionString,
            string tableName,
            IDictionary<string, DefaultColumnWriter>? loggerColumnOptions = null,
            IDictionary<string, SinglePropertyColumnWriter>? loggerPropertyColumnOptions = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider? formatProvider = null,
            LoggingLevelSwitch? levelSwitch = null,
            bool needAutoCreateTable = false,
            IConfiguration? appConfiguration = null,
            Action<CreateTableEventArgs>? onCreateTableCallback = null,
            TimeSpan? retentionTime = null)
        {
            if (sinkConfiguration is null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            if (appConfiguration is not null)
            {
                connectionString =
                    MicrosoftExtensionsConnectionStringProvider.GetConnectionString(connectionString, appConfiguration);
            }

            IDictionary<string, ColumnWriterBase>? columns = null;

            if (loggerColumnOptions is not null)
            {
                columns = new Dictionary<string, ColumnWriterBase>();

                foreach (var columnOption in loggerColumnOptions)
                {
                    var name = columnOption.Value.Name;

                    switch (name)
                    {
                        case "Level":
                            columns.Add(columnOption.Key, new LevelColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "LevelAsText":
                            columns.Add(columnOption.Key, new LevelColumnWriter(true, MySqlDbType.Text, columnOption.Value.Order));
                            break;
                        case "Timestamp":
                            columns.Add(columnOption.Key, new TimestampColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "LogEvent":
                            columns.Add(columnOption.Key, new LogEventSerializedColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "Properties":
                            columns.Add(columnOption.Key, new PropertiesColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "Message":
                            columns.Add(columnOption.Key, new MessageTemplateColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "RenderedMessage":
                            columns.Add(columnOption.Key, new RenderedMessageColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "Exception":
                            columns.Add(columnOption.Key, new ExceptionColumnWriter(order: columnOption.Value.Order));
                            break;
                        case "IdAutoIncrement":
                            columns.Add(columnOption.Key, new IdAutoIncrementColumnWriter(order: columnOption.Value.Order));
                            break;
                    }
                }
            }

            if (loggerPropertyColumnOptions is null)
            {
                var optionsLocal = GetOptions(
                    connectionString,
                    tableName,
                    columns,
                    TimeSpan.Zero,
                    formatProvider,
                    DefaultBatchSizeLimit,
                    DefaultQueueLimit,
                    false,
                    needAutoCreateTable,
                    onCreateTableCallback,
                    retentionTime);

                return sinkConfiguration.Sink(new MySqlAuditSink(optionsLocal), restrictedToMinimumLevel, levelSwitch);
            }

            columns ??= new Dictionary<string, ColumnWriterBase>();

            foreach (var columnOption in loggerPropertyColumnOptions)
            {
                columns.Add(columnOption.Key, columnOption.Value);
            }

            var optionsLocal2 = GetOptions(
               connectionString,
               tableName,
               columns,
               TimeSpan.Zero,
               formatProvider,
               DefaultBatchSizeLimit,
               DefaultQueueLimit,
               false,
               needAutoCreateTable,
               onCreateTableCallback,
               retentionTime);

            return sinkConfiguration.Sink(new MySqlAuditSink(optionsLocal2), restrictedToMinimumLevel, levelSwitch);
        }

        internal static MySqlOptions GetOptions(
            string connectionString,
            string tableName,
            IDictionary<string, ColumnWriterBase>? columnOptions,
            TimeSpan period,
            IFormatProvider? formatProvider,
            int batchSizeLimit,
            int queueLimit,
            bool useBulkInsert,
            bool needAutoCreateTable,
            Action<CreateTableEventArgs>? onCreateTableCallback,
            TimeSpan? retentionTime = null)
        {
            var columnOptionsLocal = ClearQuotationMarksFromColumnOptions(columnOptions ?? ColumnOptions.Default);

            return new MySqlOptions
            {
                ConnectionString = connectionString,
                TableName = tableName.Replace("`", string.Empty),
                Period = period,
                FormatProvider = formatProvider,
                ColumnOptions = columnOptionsLocal,
                BatchSizeLimit = batchSizeLimit,
                QueueLimit = queueLimit,
                UseBulkInsert = useBulkInsert,
                NeedAutoCreateTable = needAutoCreateTable,
                OnCreateTable = onCreateTableCallback,
                RetentionTime = retentionTime
            };
        }


        internal static IDictionary<string, ColumnWriterBase> ClearQuotationMarksFromColumnOptions
            (IDictionary<string, ColumnWriterBase> columnOptions)
        {
            var result = new Dictionary<string, ColumnWriterBase>(columnOptions);

            foreach (var keyValuePair in columnOptions)
            {
                if (!keyValuePair.Key.Contains('`'))
                {
                    continue;
                }

                result.Remove(keyValuePair.Key);
                result[keyValuePair.Key.Replace("`", string.Empty)] = keyValuePair.Value;
            }

            return result;
        }
    }
}
