using CQRSlite.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SachaBarber.CQRS.Demo.Orders.Domain.Events;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Aggregates
{
    public class Order : AggregateRoot
    {

        public string description { get; set; }
        public string address { get; set; }
        public List<OrderItem> orderItems { get; set; }

        private void Apply(OrderCreatedEvent e)
        {
            description = e.Description;
            address = e.Address;
            orderItems = e.OrderItems;
        }


        private void Apply(OrderRenamedEvent e)
        {
            description = e.NewOrderDescription;
        }


        public void RenameOrder(string newOrderDescription)
        {
            if (string.IsNullOrEmpty(newOrderDescription)) 
                throw new ArgumentException("newOrderDescription");
            ApplyChange(new OrderRenamedEvent(Id, newOrderDescription));
        }

        private Order() { }

        public Order(
            Guid id,
            string description,
            string address,
            List<OrderItem> orderItems)
        {
            Id = id;
            ApplyChange(new OrderCreatedEvent(id, description, address, orderItems));
        }
    }
}
