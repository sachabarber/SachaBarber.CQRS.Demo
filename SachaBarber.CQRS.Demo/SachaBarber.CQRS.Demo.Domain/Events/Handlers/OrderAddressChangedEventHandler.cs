using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SachaBarber.CQRS.Demo.Orders.Domain.Bus;
using SachaBarber.CQRS.Demo.Orders.ReadModel;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Events.Handlers
{
    public class OrderAddressChangedEventHandler : IBusEventHandler<OrderAddressChangedEvent>
    {
        private readonly IReadModelRepository readModelRepository;
        private readonly IInterProcessBus interProcessBus;

        public OrderAddressChangedEventHandler(
            IReadModelRepository readModelRepository,
            IInterProcessBus interProcessBus)
        {
            this.readModelRepository = readModelRepository;
            this.interProcessBus = interProcessBus;
        }

        public Type HandlerType
        {
            get { return typeof(OrderAddressChangedEvent); }
        }

        public async void Handle(OrderAddressChangedEvent orderAddressChangedEvent)
        {
            await readModelRepository.UpdateOrderAddress(orderAddressChangedEvent.Id,
                        orderAddressChangedEvent.NewOrderAddress, orderAddressChangedEvent.Version);

            interProcessBus.SendMessage("OrderAddressChangedEvent");

        }
    }
}
