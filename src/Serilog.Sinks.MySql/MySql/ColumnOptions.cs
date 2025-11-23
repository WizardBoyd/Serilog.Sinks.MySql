using MySqlConnector;
using Serilog.Sinks.MySql.MySql.ColumnWriters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.MySql.MySql
{

    public class ColumnOptions
    {
        public static IDictionary<string, ColumnWriterBase> Default => new Dictionary<string, ColumnWriterBase>()
        {
            {DefaultColumnNames.RenderedMesssage,new RenderedMessageColumnWriter()},
            {DefaultColumnNames.MessageTemplate, new MessageTemplateColumnWriter()},
            {DefaultColumnNames.Level, new LevelColumnWriter()},
            {DefaultColumnNames.Timestamp, new TimestampColumnWriter()},
            {DefaultColumnNames.Exception, new ExceptionColumnWriter()},
            {DefaultColumnNames.LogEventSerialized, new LogEventSerializedColumnWriter()}
        };

    }


    public static class DefaultColumnNames
    {
        public const string RenderedMesssage = "message";

        public const string MessageTemplate = "message_template";

        public const string Level = "level";

        public const string Timestamp = "timestamp";

        public const string Exception = "exception";

        public const string LogEventSerialized = "log_event";
    }
}
