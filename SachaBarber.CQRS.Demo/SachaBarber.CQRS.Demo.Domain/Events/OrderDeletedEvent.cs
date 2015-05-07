using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CQRSlite.Events;
using SachaBarber.CQRS.Demo.Orders.Domain.Aggregates;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Events
{
    public class OrderDeletedEvent : EventBase
    {
        public OrderDeletedEvent()
        {
            
        }

        public OrderDeletedEvent(Guid id, int version)
        {
            Id = id;
            Version = version;
        }
    }
}
