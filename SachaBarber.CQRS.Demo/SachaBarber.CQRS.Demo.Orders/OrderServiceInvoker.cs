using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SachaBarber.CQRS.Demo.Orders.Commands;
using SachaBarber.CQRS.Demo.SharedCore;

namespace SachaBarber.CQRS.Demo.Orders
{
    public class OrderServiceInvoker : ServiceInvokerBase, IOrderService
    {
        public  bool SendCommand(Command command)
        {
            return this.CallService<IOrderService, bool>(proxy => proxy.SendCommand(command));
        }
    }
}
