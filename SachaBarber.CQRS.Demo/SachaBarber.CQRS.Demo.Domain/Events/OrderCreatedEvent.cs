using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CQRSlite.Events;
using SachaBarber.CQRS.Demo.Orders.Domain.Aggregates;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Events
{
    public class OrderCreatedEvent : IEvent
    {

        public readonly string Description;
        public readonly string Address;
        public readonly List<OrderItem> OrderItems;


        public OrderCreatedEvent()
        {
            
        }

        public OrderCreatedEvent(
            Guid id, 
            string description,
            string address,
            List<OrderItem> orderItems 
            )
        {
            Id = id;
            Description = description;
            Address = address;
            OrderItems = orderItems;
        }

        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
