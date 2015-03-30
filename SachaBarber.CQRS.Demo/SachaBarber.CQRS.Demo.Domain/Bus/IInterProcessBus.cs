using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Bus
{
    public interface IInterProcessBus
    {
        void SendMessage(string message);
    }
}
