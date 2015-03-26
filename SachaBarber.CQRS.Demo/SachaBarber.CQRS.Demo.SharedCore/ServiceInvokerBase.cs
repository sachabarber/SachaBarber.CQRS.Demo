using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using NLog;

using SachaBarber.CQRS.Demo.SharedCore.Exceptions;
using SachaBarber.CQRS.Demo.SharedCore.Faults;

namespace SachaBarber.CQRS.Demo.SharedCore
{
    public abstract class ServiceInvokerBase
    {
        protected static readonly ChannelFactoryManager FactoryManager = new ChannelFactoryManager();
        protected static readonly ClientSection ClientSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;

        private Logger logger = LogManager.GetLogger("ServiceInvokerBase");

        public TR CallService<TSERV, TR>(Func<TSERV, TR> requestAction)
        {
            TSERV proxy = CreateProxy<TSERV>();
            ICommunicationObject commObj = (ICommunicationObject)proxy;
            try
            {
                return requestAction(proxy);
            }
            catch (FaultException<GenericFault> gf)
            {
                logger.Error("A FaultException<GenericFault> occured", gf.InnerException);
                throw new ApplicationException("A FaultException<GenericFault> occured", gf.InnerException);
            }
            catch (FaultException<BusinessLogicFault> bf)
            {
                logger.Error("A FaultException<BusinessLogicFault> occured", bf.InnerException);
                throw new BusinessLogicException(bf.Message);
            }
            catch (Exception ex)
            {
                logger.Error("A Exception occured", ex);
                throw new Exception("A Exception occured", ex);
            }
            finally
            {
                try
                {
                    if (commObj.State != CommunicationState.Faulted)
                    {
                        commObj.Close();
                    }
                }
                catch
                {
                    commObj.Abort();
                }
            }
        }

        public void CallService<TSERV>(Action<TSERV> requestAction)
        {
            TSERV proxy = CreateProxy<TSERV>();
            ICommunicationObject commObj = (ICommunicationObject)proxy;
            try
            {
                requestAction(proxy);
            }
            catch (FaultException<GenericFault> gf)
            {
                logger.Error("A FaultException<GenericFault> occured", gf.InnerException);
                throw new ApplicationException("A FaultException<GenericFault> occured", gf.InnerException);
            }
            catch (FaultException<BusinessLogicFault> bf)
            {
                logger.Error("A FaultException<BusinessLogicFault> occured", bf.InnerException);
                throw new BusinessLogicException(bf.Message);
            }
            catch (Exception ex)
            {
                logger.Error("A Exception occured", ex);
                throw new Exception("A Exception occured", ex);
            }
            finally
            {
                try
                {
                    if (commObj.State != CommunicationState.Faulted)
                    {
                        commObj.Close();
                    }
                }
                catch
                {
                    commObj.Abort();
                }
            }
        }

       

        protected static KeyValuePair<string, string> GetEndpointNameAddressPair(Type serviceContractType)
        {
            var configException = new ConfigurationErrorsException(string.Format("No client endpoint found for type {0}. Please add the section <client><endpoint name=\"myservice\" address=\"http://address/\" binding=\"basicHttpBinding\" contract=\"{0}\"/></client> in the config file.", serviceContractType));
            if (((ClientSection == null) || (ClientSection.Endpoints == null)) || (ClientSection.Endpoints.Count < 1))
            {
                throw configException;
            }
            foreach (ChannelEndpointElement element in ClientSection.Endpoints)
            {
                if (element.Contract == serviceContractType.ToString())
                {
                    return new KeyValuePair<string, string>(element.Name, element.Address.AbsoluteUri);
                }
            }
            throw configException;
        }

        private TService CreateProxy<TService>()
        {
            var endpointNameAddressPair = GetEndpointNameAddressPair(typeof(TService));
            Type[] methodParamTypes = new Type[2] { typeof(string), typeof(string) };
            MethodInfo method = FactoryManager.GetType().GetMethod("CreateChannel", methodParamTypes);
            MethodInfo generic = method.MakeGenericMethod(typeof(TService));
            TService proxy = (TService)generic.Invoke(FactoryManager, new object[] { endpointNameAddressPair.Key, endpointNameAddressPair.Value });
            return proxy;
        }
    }

}
