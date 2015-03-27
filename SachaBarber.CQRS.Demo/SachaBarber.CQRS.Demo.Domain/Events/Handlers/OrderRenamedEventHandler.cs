using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SachaBarber.CQRS.Demo.Orders.Domain.Bus;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Events.Handlers
{
    public class OrderRenamedEventHandler : IBusEventHandler<OrderRenamedEvent>
    {
        public Type HandlerType
        {
            get { return typeof(OrderRenamedEvent); }
        }

        public void Handle(OrderRenamedEvent orderRenamedEvent)
        {
            //TODO : This should update readmodel, and then publish 
            //       back out to UI using rabbitMQ

        }
    }
}
