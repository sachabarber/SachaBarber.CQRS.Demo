using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;
using SachaBarber.CQRS.Demo.Orders.Commands;
using SachaBarber.CQRS.Demo.Orders.Domain.Commands;
using SachaBarber.CQRS.Demo.Orders.ReadModel;
using SachaBarber.CQRS.Demo.Orders.ReadModel.Models;
using SachaBarber.CQRS.Demo.SharedCore.Exceptions;
using SachaBarber.CQRS.Demo.SharedCore.WCF;

namespace SachaBarber.CQRS.Demo.Orders.Domain
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorHandlerBehavior]
    public class OrderService : IOrderService
    {
        private readonly OrderCommandHandlers commandHandlers;
        private readonly IReadModelRepository readModelRepository;

        public OrderService(OrderCommandHandlers commandHandlers, IReadModelRepository readModelRepository)
        {
            this.commandHandlers = commandHandlers;
            this.readModelRepository = readModelRepository;
        }


        public async Task<bool> SendCommand(Command command)
        {
            await Task.Run(() =>
            {
                var meth = (from m in typeof(OrderCommandHandlers)
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            let prms = m.GetParameters()
                            where prms.Count() == 1 && prms[0].ParameterType == command.GetType()
                            select m).FirstOrDefault();

                if (meth == null)
                    throw new BusinessLogicException(
                        string.Format("Handler for {0} could not be found", command.GetType().Name));

                meth.Invoke(commandHandlers, new[] { command });
            });
            return true;
        }

        public async System.Threading.Tasks.Task<List<StoreItem>> GetAllStoreItems()
        {
            var storeItems = await readModelRepository.GetAll<StoreItem>();
            return storeItems;
        }

        public async System.Threading.Tasks.Task<List<Order>> GetAllOrders()
        {
            var orders = await readModelRepository.GetAll<Order>();
            return orders;
        }

        public async System.Threading.Tasks.Task<Order> GetOrder(Guid orderId)
        {
            var order = await readModelRepository.GetOrder(orderId);
            return order;
        }

    }
}
