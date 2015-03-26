using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace SachaBarber.CQRS.Demo.SharedCore.WCF
{
    public class ErrorHandler : IErrorHandler, IServiceBehavior
    {

        private Logger logger = LogManager.GetLogger("ServiceInvokerBase");

        public bool HandleError(Exception error)
        {
#if DEBUG
            if (error is FaultException)
            {
                FaultException faultException = (FaultException)error;

                if (faultException.Code.SubCode != null && faultException.Code.SubCode.Name == "DeserializationFailed")
                {
                    System.Diagnostics.Debug.Assert(false, "Deserialization.....Arrrrrrrh Error Fix it now.");
                }
            }
            else if (error is CommunicationException)
            {
                CommunicationException communicationException = (CommunicationException)error;
                if (communicationException.TargetSite.Name == "SerializeParameterPart")
                {
                    System.Diagnostics.Debug.Assert(false,
                                                    string.Format("Serialization.....Arrrrrrrh Error Fix it now. {0}",
                                                                  communicationException.InnerException.Message));
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, string.Format("WCF doesn't like you. {0}", error.Message));
            }
#endif
            logger.Error(error);
            return false;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {

        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler = new ErrorHandler();
            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                if (channelDispatcher != null)
                {
                    channelDispatcher.ErrorHandlers.Add(errorHandler);
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {


        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ErrorHandlerBehavior : Attribute, IDispatchMessageInspector,
        IClientMessageInspector, IEndpointBehavior, IServiceBehavior
    {
        private IErrorHandler GetInstance()
        {
            return new ErrorHandler();
        }

        #region IDispatchMessageInspector

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            //No need to do anything else

        }

        #endregion

        #region IClientMessageInspector

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            //No need to do anything else
        }

        #endregion

        #region IEndpointBehavior

        public void Validate(ServiceEndpoint endpoint)
        {
            bool isMex = endpoint.Contract.Name.Equals("IMetadataExchange")
                         && endpoint.Contract.Namespace.Equals("http://schemas.microsoft.com/2006/04/mex");

            if (!isMex)
            {

                foreach (OperationDescription description in endpoint.Contract.Operations)
                {
                    if (description.Faults.Count == 0)
                    {
                        //   throw new InvalidOperationException("FaultContractAttribute not found on this method");
                    }
                }
            }

        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var channelDispatcher = endpointDispatcher.ChannelDispatcher;
            if (channelDispatcher == null) return;
            foreach (var ed in channelDispatcher.Endpoints)
            {
                var inspector = new ErrorHandlerBehavior();
                ed.DispatchRuntime.MessageInspectors.Add(inspector);
            }
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var inspector = new ErrorHandlerBehavior();
            clientRuntime.MessageInspectors.Add(inspector);
        }

        #endregion

        #region IServiceBehavior


        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandlerInstance = GetInstance();
            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
            {
                dispatcher.ErrorHandlers.Add(errorHandlerInstance);
            }
        }

        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ServiceEndpoint endpoint in serviceDescription.Endpoints)
            {
                if (endpoint.Contract.Name.Equals("IMetadataExchange") &&
                    endpoint.Contract.Namespace.Equals("http://schemas.microsoft.com/2006/04/mex"))
                    continue;

                foreach (OperationDescription description in endpoint.Contract.Operations)
                {
                    if (description.Faults.Count == 0)
                    {
                        //   throw new InvalidOperationException("FaultContractAttribute not found on this method");
                    }
                }
            }
        }
        #endregion
    }
}
