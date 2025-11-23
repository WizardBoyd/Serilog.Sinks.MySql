

namespace Serilog.Sinks.MySql.MySql.Asnyc
{
    public readonly struct AsyncEventInvocator<TEventArgs>
    {
        private readonly Action<TEventArgs>? handler;

        private readonly Func<TEventArgs, Task>? asyncHandler;

        public AsyncEventInvocator(Action<TEventArgs>? handler, Func<TEventArgs, Task>? asyncHandler)
        {
            this.handler = handler;
            this.asyncHandler = asyncHandler;
        }

        public bool WrapsHandler(Action<TEventArgs> handler)
        {
            return handler == this.handler;
        }

        public bool WrapsHandler(Func<TEventArgs, Task> handler)
        {
            return handler == this.asyncHandler;
        }

        public Task Invoke(TEventArgs args)
        {
            if(this.handler != null)
            {
                this.handler.Invoke(args);
                return Task.CompletedTask;
            }

            if(this.asyncHandler != null)
            {
                return this.asyncHandler.Invoke(args);
            }
            throw new InvalidOperationException();
        }
    }
}
