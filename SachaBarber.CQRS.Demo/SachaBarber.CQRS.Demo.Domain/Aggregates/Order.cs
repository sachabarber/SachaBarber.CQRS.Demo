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
        private string description;
        private string address;
        private bool isDeleted;
        private List<OrderItem> orderItems;

        private void Apply(OrderCreatedEvent e)
        {
            Version = e.Version;
            description = e.Description;
            address = e.Address;
            isDeleted = false;
            orderItems = e.OrderItems;
        }

        private void Apply(OrderAddressChangedEvent e)
        {
            Version = e.Version;
            address = e.NewOrderAddress;
        }

        private void Apply(OrderDeletedEvent e)
        {
            Version = e.Version;
            isDeleted = true;
        }

        public void ChangeAddress(string newAddress)
        {
            if (string.IsNullOrEmpty(newAddress))
                throw new ArgumentException("newAddress");
            ApplyChange(new OrderAddressChangedEvent(Id, newAddress,Version));
        }

        public void Delete()
        {
            ApplyChange(new OrderDeletedEvent(Id, Version));
        }


        private Order() { }

        public Order(
            Guid id,
            int version,
            string description,
            string address,
            List<OrderItem> orderItems
            )
        {
            Id = id;
            ApplyChange(new OrderCreatedEvent(id, description, address, orderItems, version));
        }
    }
}
