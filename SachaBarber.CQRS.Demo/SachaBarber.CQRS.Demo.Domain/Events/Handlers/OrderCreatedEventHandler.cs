using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SachaBarber.CQRS.Demo.Orders.Domain.Bus;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Events.Handlers
{
    public class OrderCreatedEventHandler : IBusEventHandler<OrderCreatedEvent>
    {
        public Type HandlerType
        {
            get { return typeof (OrderCreatedEvent); }
        }

        public void Handle(OrderCreatedEvent orderCreatedEvent)
        {
            //TODO : This should update readmodel, and then publish 
            //       back out to UI using rabbitMQ

            
        }
    }
}
