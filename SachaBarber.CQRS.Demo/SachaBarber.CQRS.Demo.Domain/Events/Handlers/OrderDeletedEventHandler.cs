using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SachaBarber.CQRS.Demo.Orders.Domain.Bus;
using SachaBarber.CQRS.Demo.Orders.ReadModel;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Events.Handlers
{
    public class OrderDeletedEventHandler : IBusEventHandler<OrderDeletedEvent>
    {
        private readonly IReadModelRepository readModelRepository;
        private readonly IInterProcessBus interProcessBus;

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
            await readModelRepository.DeleteOrder(orderDeletedEvent.Id);
            interProcessBus.SendMessage("OrderDeletedEvent");
        }
    }
}
