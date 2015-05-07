using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CQRSlite.Events;
using SachaBarber.CQRS.Demo.Orders.Domain.Aggregates;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Events
{
    public class OrderCreatedEvent : EventBase
    {
        public OrderCreatedEvent()
        {
            
        }

        public OrderCreatedEvent(
            Guid id, 
            string description,
            string address,
            List<OrderItem> orderItems,
            int version
            )
        {
            Id = id;
            Version = version;
            Description = description;
            Address = address;
            OrderItems = orderItems;

        }

        public string Description { get; private set; }
        public string Address { get; private set; }
        public List<OrderItem> OrderItems { get; private set; }
    }
}
