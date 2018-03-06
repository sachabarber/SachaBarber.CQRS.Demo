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
        public async Task<TR> CallService<TR>(Func<IOrderService, Task<TR>> requestAction)
        {
            OrderServiceClient proxy = null;
            try
            {
                proxy = new OrderServiceClient();
                return await requestAction(proxy);
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
            finally
            {
                if (proxy != null)
                {
                    proxy.Close();
                }
            }
        }
    }
}
