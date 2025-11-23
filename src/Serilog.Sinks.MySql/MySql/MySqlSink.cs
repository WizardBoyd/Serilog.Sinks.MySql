
using Serilog.Core;
using Serilog.Events;


namespace Serilog.Sinks.MySql.MySql
{
    public class MySqlSink : IBatchedLogEventSink
    {
       private readonly SinkHelper sinkHelper;

        public MySqlSink(MySqlOptions options)
        {
            this.sinkHelper = new SinkHelper(options);
        }

        public async Task EmitBatchAsync(IReadOnlyCollection<LogEvent> batch)
        {
            await sinkHelper.Emit(batch);
        }

        public async Task OnEmptyBatchAsync()
        {
            await sinkHelper.EmptyEmit();
        }
    }

}
