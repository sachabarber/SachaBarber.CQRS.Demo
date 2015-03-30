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
        Task<bool> SendCommand(Command command);

        [OperationContract]
        Task<List<StoreItem>> GetAllStoreItems();

        [OperationContract]
        Task<List<Order>> GetAllOrders();

        [OperationContract]
        Task<Order> GetOrder(Guid orderId);

    }
}
