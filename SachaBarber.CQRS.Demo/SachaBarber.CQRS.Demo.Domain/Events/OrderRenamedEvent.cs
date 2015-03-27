using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CQRSlite.Events;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Events
{
    public class OrderRenamedEvent : IEvent
    {
        public readonly string NewOrderDescription;

        public OrderRenamedEvent()
        {
            
        }

        public OrderRenamedEvent(Guid id, string newOrderDescription)
        {
            Id = id;
            NewOrderDescription = newOrderDescription;
        }

        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
