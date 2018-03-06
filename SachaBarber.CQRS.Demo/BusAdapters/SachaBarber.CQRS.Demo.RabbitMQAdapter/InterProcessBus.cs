using System.Configuration;
using System.Text;
using RabbitMQ.Client;
using SachaBarber.CQRS.Demo.SharedCore.Services;

namespace SachaBarber.CQRS.Demo.RabbitMQAdapter
{
    public class InterProcessBus : IInterProcessBus
    {

        private readonly string busName;
        private readonly string connectionString;

        public InterProcessBus()
        {
            this.busName = "InterProcessBus";
            this.connectionString = ConfigurationManager.AppSettings["RabbitMqHost"];
        }

        public void SendMessage(string message)
        {
            var factory = new ConnectionFactory() { HostName = connectionString };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var bytes = Encoding.ASCII.GetBytes(message);
                    channel.ExchangeDeclare(busName, "fanout");
                    channel.BasicPublish(busName, string.Empty, null, bytes);
                }
            }
        }
    }
}
