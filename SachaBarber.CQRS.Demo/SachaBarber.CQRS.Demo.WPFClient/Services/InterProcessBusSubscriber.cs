using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Raven.Imports.Newtonsoft.Json.Linq;

namespace SachaBarber.CQRS.Demo.WPFClient.Services
{
    public class InterProcessBusSubscriber : IInterProcessBusSubscriber, IDisposable
    {

        private readonly string busName;
        private readonly string connectionString;
        private CancellationTokenSource cancellationToken;
        private Task workerTask;
        private Subject<string> eventsSubject = new Subject<string>();

        public InterProcessBusSubscriber()
        {
            this.busName = "InterProcessBus";
            this.connectionString = ConfigurationManager.AppSettings["RabbitMqHost"];
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
            cancellationToken.Cancel();
            workerTask.Wait();
            workerTask.Dispose();
        }

        private void ListenForMessage()
        {
            var factory = new ConnectionFactory() { HostName = connectionString };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(busName, "fanout");

                    bool durable = true;
                    bool exclusive = false;
                    bool autoDelete = false;

                    var queue = channel.QueueDeclare(Assembly.GetEntryAssembly().GetName().Name,
                        durable, exclusive, autoDelete, null);
                    channel.QueueBind(queue.QueueName, busName, string.Empty);
                    var consumer = new QueueingBasicConsumer(channel);

                    channel.BasicConsume(queue.QueueName, false, string.Empty, consumer);

                    while (true)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;
                        BasicDeliverEventArgs ea;
                        consumer.Queue.Dequeue(10, out ea);

                        if (ea == null)
                            continue;

                        var message = Encoding.ASCII.GetString(ea.Body);
                        Task.Run(async () =>
                            {
                                await Task.Run(() =>
                                    {
                                        eventsSubject.OnNext(message);
                                    });
                            });
                        channel.BasicAck(ea.DeliveryTag, false);
                    }

                }

            }
        }


        public IObservable<string> GetEventStream()
        {
            return eventsSubject.AsObservable();
        }
    }
}
