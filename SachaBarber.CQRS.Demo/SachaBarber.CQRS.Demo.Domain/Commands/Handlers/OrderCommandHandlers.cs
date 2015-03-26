using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CQRSlite.Commands;
using CQRSlite.Domain;

using SachaBarber.CQRS.Demo.Orders.Commands;
using SachaBarber.CQRS.Demo.Orders.Domain.Aggregates;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Commands
{
    public class OrderCommandHandlers : ICommandHandler<CreateOrderCommand>,
                                        ICommandHandler<RenameOrderCommand>
    {
        private readonly ISession _session;

        public OrderCommandHandlers(ISession session)
        {
            _session = session;
        }

        public void Handle(CreateOrderCommand command)
        {
            var item = new Order(command.Id, command.OrderDescription);
            _session.Add(item);
            _session.Commit();
        }


        public void Handle(RenameOrderCommand command)
        {
            Order item = _session.Get<Order>(command.Id, command.ExpectedVersion);
            item.RenameOrder(command.NewOrderDescription);
            _session.Commit();
        }

    }
}
