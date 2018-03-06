using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SachaBarber.CQRS.Demo.SharedCore.Services;
using SachaBarber.CQRS.Demo.Orders.Domain.Bus;
using SachaBarber.CQRS.Demo.Orders.ReadModel;
using NLog;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Events.Handlers
{
    public class OrderDeletedEventHandler : IBusEventHandler<OrderDeletedEvent>
    {
        private readonly IReadModelRepository readModelRepository;
        private readonly IInterProcessBus interProcessBus;
        private Logger logger = LogManager.GetLogger("OrderDeletedEventHandler");

        public OrderDeletedEventHandler(
            IReadModelRepository readModelRepository,
            IInterProcessBus interProcessBus)
        {
            this.readModelRepository = readModelRepository;
            this.interProcessBus = interProcessBus;
        }

        public Type HandlerType
        {
            get { return typeof(OrderDeletedEvent); }
        }

        public async void Handle(OrderDeletedEvent orderDeletedEvent)
        {
            logger.Info("Handling OrderDeletedEvent event: {0} ({1})", orderDeletedEvent.Id, orderDeletedEvent.Version);
            await readModelRepository.DeleteOrder(orderDeletedEvent.Id);
            interProcessBus.SendMessage("OrderDeletedEvent");
        }
    }
}
