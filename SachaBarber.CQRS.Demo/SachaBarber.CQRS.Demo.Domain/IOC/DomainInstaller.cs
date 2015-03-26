using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

using CQRSlite.Bus;
using CQRSlite.Cache;
using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Events;

using NServiceBus;
using System;
using System.Reflection;

using SachaBarber.CQRS.Demo.Orders.Domain.Commands;
using SachaBarber.CQRS.Demo.Orders.Domain.EventStore;
using SachaBarber.CQRS.Demo.SharedCore.ExtensionMethods;
using SachaBarber.CQRS.Demo.SharedCore.IOC;

namespace SachaBarber.CQRS.Demo.Orders.Domain.IOC
{


    public class DomainInstaller : IWindsorInstaller
    {
        private readonly ILifestyleApplier lifestyleApplier;

        public DomainInstaller(ILifestyleApplier lifestyleApplier)
        {
            this.lifestyleApplier = lifestyleApplier;
        }



        // CRQRS LITE setup
        // CRQRS LITE setup
        // CRQRS LITE setup
        // CRQRS LITE setup
        // CRQRS LITE setup

        //  ObjectFactory.Initialize(x =>
        //            {
        //                      x.For<InProcessBus>().Singleton().Use<InProcessBus>();
        //                      x.For<ICommandSender>().Use(y => y.GetInstance<InProcessBus>());
        //                      x.For<IEventPublisher>().Use(y => y.GetInstance<InProcessBus>());
        //                      x.For<IHandlerRegistrar>().Use(y => y.GetInstance<InProcessBus>());
        //                      x.For<ISession>().HybridHttpOrThreadLocalScoped().Use<Session>();
        //                      x.For<IEventStore>().Singleton().Use<InMemoryEventStore>();
        //                x.For<IRepository>().HybridHttpOrThreadLocalScoped().Use(y =>
        //                                                                         new CacheRepository(
        //                                                                             new Repository(y.GetInstance<IEventStore>(),
        //                                                                                            y.GetInstance<IEventPublisher>()),
        //                                                                             y.GetInstance<IEventStore>()));

        //                x.Scan(s =>
        //                {
        //                    s.TheCallingAssembly();
        //                    s.AssemblyContainingType<ReadModelFacade>();
        //                    s.Convention<FirstInterfaceConvention>();
        //                });
        //            });
        //return ObjectFactory.Container;


        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel, true));

            // PerCall setup
            container.AddFacility<WcfFacility>()
            .Register(

                //wcf client invokers
                Component.For<IOrderService>().ImplementedBy<OrderService>().LifeStyle.ApplyLifeStyle(lifestyleApplier),

                //CQRSLite stuff
                Component.For<OrderCommandHandlers>().LifeStyle.ApplyLifeStyle(lifestyleApplier),
                Component.For<IEventPublisher, ICommandSender, IHandlerRegistrar>().ImplementedBy<InProcessBus>().LifeStyle.ApplyLifeStyle(lifestyleApplier),
                Component.For<ISession>().ImplementedBy<Session>().LifeStyle.ApplyLifeStyle(lifestyleApplier),
                Component.For<IEventStore>().ImplementedBy<InMemoryEventStore>().LifeStyle.Singleton,


                Component.For<IRepository>().UsingFactoryMethod(
                    kernel =>
                    {
                        return
                            new CacheRepository(
                                new Repository(kernel.Resolve<IEventStore>(), kernel.Resolve<IEventPublisher>()),
                                kernel.Resolve<IEventStore>());
                    })

            );

            //if (lifestyleApplier is WcfLifestyleApplier)
            //{
            //    container.Register(Component.For<IBus>().UsingFactoryMethod(NSBInstaller.CreateBusFactory).LifeStyle.Singleton);
            //}




        }

    }
}
