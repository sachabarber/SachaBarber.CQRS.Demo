using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using SachaBarber.CQRS.Demo.Orders.Commands;
using SachaBarber.CQRS.Demo.SharedCore.Faults;

namespace SachaBarber.CQRS.Demo.Orders
{
    [ServiceContract]
    [StandardFaults]
    public interface IOrderService
    {
        [OperationContract]
        Task<bool> SendCommand(Command command);
    }
}
