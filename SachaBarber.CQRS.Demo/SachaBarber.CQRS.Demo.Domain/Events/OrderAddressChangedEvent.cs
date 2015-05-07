using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CQRSlite.Events;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Events
{
    public class OrderAddressChangedEvent : EventBase
    {
        public readonly string NewOrderAddress;

        public OrderAddressChangedEvent()
        {
            
        }

        public OrderAddressChangedEvent(Guid id, string newOrderAddress, int  version)
        {
            Id = id;
            NewOrderAddress = newOrderAddress;
            Version = version;
        }
    }
}
