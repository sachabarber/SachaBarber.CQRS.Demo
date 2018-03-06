using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using System.Configuration;
using System.Reactive.Linq;
using System.Messaging;
using NLog;

namespace SachaBarber.CQRS.Demo.MSMQAdapter
{
    public class BusSubscriber : SharedCore.Services.IInterProcessBusSubscriber, IDisposable
    {
        private MessageQueue _messageQueue;
        private readonly string queueName;
        private CancellationTokenSource cancellationToken;
        private Task workerTask;
        private ISubject<string> eventsSubject = new Subject<string>();
        private IObservable<Message> _obs;
        private IDisposable _subscription;
        private Logger logger = LogManager.GetLogger("MSMQAdapter.BusSubscriber");

        public BusSubscriber()
        {
            queueName = ConfigurationManager.AppSettings["MSMQueueName"];
            StartMessageListener();
        }

        private void StartMessageListener()
        {
            cancellationToken = new CancellationTokenSource();
            workerTask = Task.Factory.StartNew(() => ListenForMessage(), cancellationToken.Token);
        }

        public void Dispose()
        {
            CancelWorkerTask();
        }

        private void CancelWorkerTask()
        {
            if (workerTask == null) return;
            logger.Info("BusSubscriber task cancelled.", queueName);
            cancellationToken.Cancel();
            workerTask.Wait();
            workerTask.Dispose();
            _subscription.Dispose();
            _messageQueue.Dispose();
            _obs = null;
        }

        private void ListenForMessage()
        {
            logger.Info("Listening on queue {0} for messages.", queueName);
            _messageQueue = new MessageQueue(queueName);
            _messageQueue.ReceiveCompleted += ReadQueue;
            _messageQueue.BeginReceive();
        }

        private void ReadQueue(object sender, ReceiveCompletedEventArgs args)
        {
            var msg = _messageQueue.EndReceive(args.AsyncResult);
            try
            {
                msg.Formatter = new XmlMessageFormatter(new string[] { "System.String,mscorlib" });
                logger.Info("Message received: {0}", msg.Body.ToString());
            }
            catch (Exception e)
            {
                logger.Error("Error occurred when message received", e);
                return;
            }

            var message = msg.Body.ToString();
            Task.Run(async () =>
            {
                await Task.Run(() =>
                {
                    logger.Info("Publishing method to Rx stream");
                    eventsSubject.OnNext(message);
                });
            });

            _messageQueue.BeginReceive();
        }

        public IObservable<string> GetEventStream()
        {
            return eventsSubject.AsObservable();
        }
    }
}
