using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CQRSlite.Events;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Events
{
    public class OrderCreatedEvent : IEvent
    {
        public readonly string OrderDescription;
        public OrderCreatedEvent(Guid id, string orderDescription)
        {
            Id = id;
            OrderDescription = orderDescription;
        }

        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
