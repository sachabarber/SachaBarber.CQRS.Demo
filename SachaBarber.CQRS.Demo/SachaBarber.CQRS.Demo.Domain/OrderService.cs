using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;
using SachaBarber.CQRS.Demo.Orders.Commands;
using SachaBarber.CQRS.Demo.Orders.Domain.Commands;
using SachaBarber.CQRS.Demo.SharedCore.Exceptions;
using SachaBarber.CQRS.Demo.SharedCore.WCF;

namespace SachaBarber.CQRS.Demo.Orders.Domain
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorHandlerBehavior]
    public class OrderService : IOrderService
    {
        private readonly OrderCommandHandlers commandHandlers;

        public OrderService(OrderCommandHandlers commandHandlers)
        {
            this.commandHandlers = commandHandlers;
        }

        public async Task<bool> SendCommand(Command command)
        {
            //var meth = (from m in typeof(OrderCommandHandlers).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            //            let prms = m.GetParameters()
            //            where prms.Count() == 1 && prms[0].ParameterType == command.GetType()
            //            select m).FirstOrDefault();

            //if (meth == null)
            //    throw new BusinessLogicException(string.Format("Handler for {0} could not be found", command.GetType().Name));

            //meth.Invoke(commandHandlers, new[] { command });
            return true;
        }
    }
}
