using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using SachaBarber.CQRS.Demo.Orders.Commands;
using SachaBarber.CQRS.Demo.SharedCore.Faults;
using SachaBarber.CQRS.Demo.Orders.ReadModel.Models;

namespace SachaBarber.CQRS.Demo.Orders
{
    [ServiceContract]
    [StandardFaults]
    public interface IOrderService
    {
        [OperationContract]
        Task<bool> SendCommandAsync(Command command);

        [OperationContract]
        Task<List<StoreItem>> GetAllStoreItemsAsync();

        [OperationContract]
        Task<List<Order>> GetAllOrdersAsync();

        [OperationContract]
        Task<Order> GetOrderAsync(Guid orderId);

    }
}
