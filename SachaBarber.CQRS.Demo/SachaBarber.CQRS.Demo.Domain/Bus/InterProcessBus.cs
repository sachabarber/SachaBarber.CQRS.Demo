using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Bus
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
