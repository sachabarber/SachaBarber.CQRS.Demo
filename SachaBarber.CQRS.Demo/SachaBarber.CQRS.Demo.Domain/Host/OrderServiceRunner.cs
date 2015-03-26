using System;
using System.ServiceModel.Dispatcher;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System.ServiceModel;

using SachaBarber.CQRS.Demo.Orders.Domain.IOC;
using SachaBarber.CQRS.Demo.SharedCore;
using SachaBarber.CQRS.Demo.SharedCore.ExtensionMethods;
using SachaBarber.CQRS.Demo.SharedCore.IOC;
using NLog;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Host
{
    class OrderServiceRunner
    {
        private ServiceHostBase dealingServiceHost;
        private ServiceHostBase dealingSearchServiceHost;
        private WindsorContainer container = new WindsorContainer();
        private Logger logger = LogManager.GetLogger("OrderServiceRunner");
        
        public void Start()
        {

            logger.Info("Starting SachaBarber CQRS Demo OrderService");
            
            try
            {
                container.Install(new IWindsorInstaller[] { new DomainInstaller(new WcfLifestyleApplier()) });
                container.CheckForPotentiallyMisconfiguredComponents();

                CreateServiceHost<IOrderService>(ref dealingServiceHost, "OrderService");


                //hook up unhandled exception listeners
                WcfExceptionHandler wcfExceptionHandler = new WcfExceptionHandler();
                ExceptionHandler.AsynchronousThreadExceptionHandler = wcfExceptionHandler;
                ExceptionHandler.TransportExceptionHandler = wcfExceptionHandler;
            }
            catch (Exception ex)
            {
                logger.Error("Error stating SachaBarber CQRS Demo OrderService", ex);
                throw;
            }
        }


        private void CreateServiceHost<T>(ref ServiceHostBase host, string message)
        {
            host = new DefaultServiceHostFactory(container.Kernel)
                    .CreateServiceHost(typeof(T).AssemblyQualifiedName, new Uri[0]);

           // Hook on to the service host faulted events.
           host.Faulted += new WeakEventHandler<EventArgs>(OnServiceFaulted).Handler;

            // Open the ServiceHostBase to create listeners and start listening for messages.
           host.Open();
           logger.Info(string.Format("SachaBarber CQRS Demo {0} Started", message));
        }
       

        public void OnServiceFaulted(object sender, EventArgs e)
        {
            logger.Error("SachaBarber CQRS Demo OrderService faulted");
            Environment.Exit(-1);
        }

        public void Stop()
        {
            try
            {
                this.dealingServiceHost.Close();
                this.dealingSearchServiceHost.Close();
            }
            catch (Exception)
            {
                this.dealingServiceHost.Abort();
                this.dealingSearchServiceHost.Abort();
                throw;
            }
        }
    }
}
