

namespace Serilog.Sinks.MySql.MySql.Asnyc
{
    public sealed class AsyncEvent<TEventArgs> where TEventArgs : System.EventArgs
    {
        private readonly List<AsyncEventInvocator<TEventArgs>> handlers = new();

        private ICollection<AsyncEventInvocator<TEventArgs>> handlersForInvoke;

        public bool HasHandlers { get; private set; }

        public AsyncEvent()
        {
            this.handlersForInvoke = this.handlers;
        }

        public void AddHandler(Func<TEventArgs, Task>? handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock (this.handlers)
            {
                this.handlers.Add(new AsyncEventInvocator<TEventArgs>(null, handler));
                this.HasHandlers = true;
                this.handlersForInvoke = new List<AsyncEventInvocator<TEventArgs>>(this.handlers);
            }
        }

        public void AddHandler(Action<TEventArgs>? handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock (this.handlers)
            {
                this.handlers.Add(new AsyncEventInvocator<TEventArgs>(handler, null));
                this.HasHandlers = true;
                this.handlersForInvoke = new List<AsyncEventInvocator<TEventArgs>>(this.handlers);
            }
        }

        public void RemoveHandler(Func<TEventArgs, Task>? handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock (this.handlers)
            {
                this.handlers.RemoveAll(h => h.WrapsHandler(handler));
                this.HasHandlers = this.handlers.Count > 0;
                this.handlersForInvoke = new List<AsyncEventInvocator<TEventArgs>>(this.handlers);
            }
        }

        public void RemoveHandler(Action<TEventArgs> handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock (this.handlers)
            {
                this.handlers.RemoveAll(h => h.WrapsHandler(handler));
                this.HasHandlers = this.handlers.Count > 0;
                this.handlersForInvoke = new List<AsyncEventInvocator<TEventArgs>>(this.handlers);
            }
        }

        public async Task Invoke(TEventArgs eventArgs)
        {
            if (!this.HasHandlers)
            {
                return;
            }

            // Adding or removing handlers will produce a new list instance all the time.
            // So locking here is not required since only the reference to an immutable list
            // of handlers is used.
            var handlers = this.handlersForInvoke;
            foreach (var handler in handlers)
            {
                await handler.Invoke(eventArgs).ConfigureAwait(false);
            }
        }

        public async Task TryInvokeAsync(TEventArgs eventArgs, ILogger logger)
        {
            if (eventArgs is null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            try
            {
                await this.Invoke(eventArgs).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                logger.Warning(exception, "Error while invoking event with arguments of type {Type}", typeof(TEventArgs));
            }
        }
    }
}
