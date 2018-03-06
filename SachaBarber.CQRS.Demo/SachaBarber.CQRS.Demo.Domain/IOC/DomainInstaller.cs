using System;
using System.Linq;
using System.Reflection;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

using CQRSlite.Bus;
using CQRSlite.Cache;
using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Events;
using SachaBarber.CQRS.Demo.Orders.Domain.Bus;
using SachaBarber.CQRS.Demo.Orders.Domain.Commands;
using SachaBarber.CQRS.Demo.Orders.Domain.Events.Handlers;
using SachaBarber.CQRS.Demo.Orders.Domain.EventStore;
using SachaBarber.CQRS.Demo.Orders.ReadModel;
using SachaBarber.CQRS.Demo.MSMQAdapter;
using SachaBarber.CQRS.Demo.SharedCore.ExtensionMethods;
using SachaBarber.CQRS.Demo.SharedCore.IOC;
using SachaBarber.CQRS.Demo.SharedCore.Services;
using NLog;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.Orders.Domain.IOC
{


    public class DomainInstaller : IWindsorInstaller
    {
        private readonly ILifestyleApplier lifestyleApplier;
        private Logger logger = LogManager.GetLogger("DomainInstaller");

        public DomainInstaller(ILifestyleApplier lifestyleApplier)
        {
            this.lifestyleApplier = lifestyleApplier;
        }


        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel, true));

            // PerCall setup
            container.AddFacility<WcfFacility>()
                .Register(

                    //WCF services
                    Component.For<IOrderService>()
                        .ImplementedBy<OrderService>()
                        .LifeStyle.ApplyLifeStyle(lifestyleApplier),

                    //CQRSLite stuff
                    Component.For<OrderCommandHandlers>().LifeStyle.ApplyLifeStyle(lifestyleApplier),
                    Component.For<IEventPublisher>().ImplementedBy<BusEventPublisher>().LifeStyle.Singleton,
                    Component.For<IInterProcessBus>().ImplementedBy<InterProcessBus>().LifeStyle.Singleton,
                    Component.For<ISession>().ImplementedBy<Session>().LifeStyle.ApplyLifeStyle(lifestyleApplier),
                    Component.For<IEventStore>().ImplementedBy<InMemoryEventStore>().LifeStyle.Singleton,
                    Component.For<IReadModelRepository>().ImplementedBy<ReadModelRepository>().LifeStyle.Singleton,
                    Component.For<IBusEventHandler>().ImplementedBy<OrderCreatedEventHandler>()
                        .Named("OrderCreatedEventHandler").LifeStyle.Singleton,
                    Component.For<IBusEventHandler>().ImplementedBy<OrderAddressChangedEventHandler>()
                        .Named("OrderAddressChangedEventHandler").LifeStyle.Singleton,
                    Component.For<IBusEventHandler>().ImplementedBy<OrderDeletedEventHandler>()
                        .Named("OrderDeletedEventHandler").LifeStyle.Singleton,
                    Component.For<IRepository>().UsingFactoryMethod(
                        kernel =>
                        {
                            return
                                new CacheRepository(
                                    new Repository(kernel.Resolve<IEventStore>(), kernel.Resolve<IEventPublisher>()),
                                    kernel.Resolve<IEventStore>());
                        })

            );

            logger.Info("Using " + container.Resolve<IInterProcessBus>());

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            logger.Error("Unobserved task exception was thrown.", e.Exception);
        }
    }
}
