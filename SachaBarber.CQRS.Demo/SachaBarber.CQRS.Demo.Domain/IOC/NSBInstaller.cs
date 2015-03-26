//using System.IO;
//using Castle.MicroKernel.Registration;
//using Castle.Windsor;

//using NServiceBus;
//using NServiceBus.Features;
//using NServiceBus.Installation.Environments;

//using SachaBarber.CQRS.Demo.SharedCore.ExtensionMethods;
//using SachaBarber.CQRS.Demo.SharedCore.IOC;
//using NServiceBus.Persistence;

//namespace SachaBarber.CQRS.Demo.Orders.Domain.IOC
//{
//    public class NSBInstaller
//    {
//        /// <summary>
//        /// Creates NServiceBus IBus
//        /// </summary>
//        /// <returns>NServiceBus Bus</returns>
//        public static IBus CreateBusFactory()
//        {

//            // See this article to see how NServiceBus deals with IOC / Disposal
//            // http://docs.particular.net/nservicebus/nservicebus-support-for-child-containers
//            WindsorContainer nsbSpecificContainer = new WindsorContainer();
//            nsbSpecificContainer.Install(new IWindsorInstaller[] { new DomainInstaller(new TransientLifestyleApplier()) });
//            nsbSpecificContainer.CheckForPotentiallyMisconfiguredComponents();

//            //See : http://docs.particular.net/nservicebus/upgradeguides/4to5
//            var cfg = new BusConfiguration();
//            cfg.EndpointName("SachaBarber.CQRS.Demo.Orders.Domain");
//            cfg.UseContainer<WindsorBuilder>(c => c.ExistingContainer(nsbSpecificContainer));
//            cfg.Transactions().Enable();
//            cfg.DisableFeature<SecondLevelRetries>();
//            cfg.EnableFeature<Sagas>();
//            cfg.EnableInstallers();
//            cfg.Conventions().DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("SachaBarber.CQRS.Demo.Messages.NSB.Commands"));
//            cfg.Conventions().DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("SachaBarber.CQRS.Demo.Messages.NSB.Events"));
//            cfg.UseTransport<MsmqTransport>();
//            cfg.UseSerialization(typeof(XmlSerializer));
//            cfg.UsePersistence<NHibernatePersistence>()
//                .For(Storage.Sagas, Storage.Subscriptions, Storage.Timeouts, Storage.GatewayDeduplication);
//            //configuration.LoadMessageHandlers(First<HandlerB>.Then<HandlerA>().AndThen<HandlerC>());
            
//#if DEBUG
//            cfg.PurgeOnStartup(true);
//#else
//            cfg.PurgeOnStartup(false);
//#endif
//            var bus2 = Bus.Create(cfg);
//            bus2.Start();
//            return bus2;
//        }
//    }
//}
