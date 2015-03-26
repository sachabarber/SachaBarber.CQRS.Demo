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
        private string orderDescription;


        private void Apply(OrderCreatedEvent e)
        {
            orderDescription = e.OrderDescription;
        }


        private void Apply(OrderRenamedEvent e)
        {
            orderDescription = e.NewOrderDescription;
        }


        public void RenameOrder(string newOrderDescription)
        {
            if (string.IsNullOrEmpty(newOrderDescription)) throw new ArgumentException("newOrderDescription");
            ApplyChange(new OrderRenamedEvent(Id, newOrderDescription));
        }

        private Order() { }

        public Order(Guid id, string orderDescription)
        {
            Id = id;
            ApplyChange(new OrderCreatedEvent(id, orderDescription));
        }
    }
}
