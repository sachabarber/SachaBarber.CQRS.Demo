using System.Configuration;
using System.Messaging;
using NLog;
using SachaBarber.CQRS.Demo.SharedCore.Services;

namespace SachaBarber.CQRS.Demo.MSMQAdapter
{
    public class InterProcessBus : IInterProcessBus
    {
        private MessageQueue _messageQueue;
        private readonly string _queueName;
        private Logger logger = LogManager.GetLogger("MSMQAdapter.InterProcessBus");

        public InterProcessBus()
        {
            _queueName = ConfigurationManager.AppSettings["MSMQueueName"];

            logger.Info("Using queue " + _queueName);

            if (!MessageQueue.Exists(_queueName))
            {
                MessageQueue.Create(_queueName);
                logger.Info("Queue created: " + _queueName);
            }

            _messageQueue = new MessageQueue(_queueName);
        }

        public void SendMessage(string message)
        {
            logger.Info("Sending message '{0}' to queue '{1}' ...", message, _queueName);
            _messageQueue.Send(message);
        }
    }
}
