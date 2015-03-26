using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SachaBarber.CQRS.Demo.SharedCore.Exceptions;
using SachaBarber.CQRS.Demo.SharedCore.Faults;

namespace SachaBarber.CQRS.Demo.Orders
{
    public class OrderServiceInvoker
    {
        public TR CallService<TR>(Func<IOrderService, TR> requestAction)
        {
            try
            {
                OrderServiceClient proxy = new OrderServiceClient(); 
                return requestAction(proxy);
            }
            catch (FaultException<GenericFault> gf)
            {
                throw new ApplicationException("A FaultException<GenericFault> occured", gf.InnerException);
            }
            catch (FaultException<BusinessLogicFault> bf)
            {
                throw new BusinessLogicException(bf.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("A Exception occured", ex);
            }
        }
    }
}
