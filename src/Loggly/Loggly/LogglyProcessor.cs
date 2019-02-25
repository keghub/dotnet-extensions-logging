using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace EMG.Extensions.Logging.Loggly
{
    public interface ILogglyProcessor : IDisposable
    {
        void EnqueueMessage(LogglyMessage message);
    }

    public class LogglyProcessor : ILogglyProcessor
    {
        private const int MaxQueuedMessages = 1024;

        private readonly ILogglyClient _client;
        private readonly BlockingCollection<LogglyMessage> _messageQueue = new BlockingCollection<LogglyMessage>(MaxQueuedMessages);
        private readonly Thread _outputThread;

        public LogglyProcessor(ILogglyClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _outputThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true
            };

            _outputThread.Start();
        }

        public void EnqueueMessage(LogglyMessage message)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(message);
                    return;
                }
                catch (InvalidOperationException)
                {
                    try
                    {
                        _messageQueue.CompleteAdding();
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch { }
                }
            }

            _client.PublishAsync(message).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _messageQueue.CompleteAdding();

            try
            {
                _outputThread.Join(TimeSpan.FromMilliseconds(1500));
            }
            catch (TaskSchedulerException) { }
        }

        private async void ProcessLogQueue()
        {
            foreach (var item in _messageQueue.GetConsumingEnumerable())
            {
                await _client.PublishAsync(item);
            }
        }
    }
}
