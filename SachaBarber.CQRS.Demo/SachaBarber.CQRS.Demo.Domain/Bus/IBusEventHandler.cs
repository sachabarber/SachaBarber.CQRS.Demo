using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Bus
{
    public interface IBusEventHandler
    {
        Type HandlerType { get; }
    }

    public interface IBusEventHandler<T> : IBusEventHandler
    {

        void Handle(T @event);
    }
}
