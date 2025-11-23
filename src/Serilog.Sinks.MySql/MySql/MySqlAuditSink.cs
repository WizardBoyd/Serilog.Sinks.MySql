

using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.MySql.MySql
{
    /// <inheritdoc cref="ILogEventSink"/>
    /// <inheritdoc cref="IDisposable"/>
    /// <summary>
    /// Writes log events as rows in a table of MySql database using Audit logic, meaning that each row is synchronously committed
    /// and any errors that occur are propagated to the caller.
    /// </summary>
    public class MySqlAuditSink : ILogEventSink, IDisposable
    {
        private readonly SinkHelper sinkHelper;

        public MySqlAuditSink(MySqlOptions options)
        {
            this.sinkHelper = new SinkHelper(options);
        }

        public async void Emit(LogEvent logEvent)
        {
            await this.sinkHelper.Emit([logEvent]);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // This class doesn't need to dispose anything. This is just here for sink interface compatibility.
        }
    }
}
