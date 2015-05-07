using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SachaBarber.CQRS.Demo.Orders.Domain.Bus;
using SachaBarber.CQRS.Demo.Orders.ReadModel;
using SachaBarber.CQRS.Demo.Orders.ReadModel.Models;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Events.Handlers
{
    public class OrderCreatedEventHandler : IBusEventHandler<OrderCreatedEvent>
    {
        private readonly IReadModelRepository readModelRepository;
        private readonly IInterProcessBus interProcessBus;

        public OrderCreatedEventHandler(
            IReadModelRepository readModelRepository,
            IInterProcessBus interProcessBus)
        {
            this.readModelRepository = readModelRepository;
            this.interProcessBus = interProcessBus;
        }

        public Type HandlerType
        {
            get { return typeof (OrderCreatedEvent); }
        }

        public async void Handle(OrderCreatedEvent orderCreatedEvent)
        {
            await readModelRepository.AddOrder(new ReadModel.Models.Order()
            {
                OrderId = orderCreatedEvent.Id,
                Address = orderCreatedEvent.Address,
                Description = orderCreatedEvent.Description,
                Version = orderCreatedEvent.Version,
                OrderItems = orderCreatedEvent.OrderItems.Select(x =>
                    new ReadModel.Models.OrderItem()
                    {
                        OrderId = x.OrderId,
                        StoreItemId = x.StoreItemId,
                        StoreItemDescription = x.StoreItemDescription,
                        StoreItemUrl = x.StoreItemUrl
                    }).ToList()
            });

            interProcessBus.SendMessage("OrderCreatedEvent");
        }
    }
}
