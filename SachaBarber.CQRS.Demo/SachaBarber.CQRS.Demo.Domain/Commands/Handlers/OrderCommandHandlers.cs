using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using CQRSlite.Commands;
using CQRSlite.Domain;
using SachaBarber.CQRS.Demo.Orders.Commands;
using SachaBarber.CQRS.Demo.Orders.Domain.Aggregates;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Commands
{
    public class OrderCommandHandlers : ICommandHandler<CreateOrderCommand>,
                                        ICommandHandler<ChangeOrderAddressCommand>,
                                        ICommandHandler<DeleteOrderCommand>
    {
        private readonly ISession _session;
        private NLog.Logger logger = NLog.LogManager.GetLogger("OrderCommandHandlers");

        public OrderCommandHandlers(ISession session)
        {
            _session = session;
        }

        public void Handle(CreateOrderCommand command)
        {
            var item = new Order(
                command.Id, 
                command.ExpectedVersion, 
                command.Description, 
                command.Address,
                command.OrderItems.Select(x => new OrderItem()
                {
                    OrderId = x.OrderId,
                    StoreItemDescription = x.StoreItemDescription,
                    StoreItemId = x.StoreItemId,
                    StoreItemUrl = x.StoreItemUrl
                }).ToList());
            _session.Add(item);
            _session.Commit();
        }

        private T Get<T>(Guid id, int? expectedVersion = null) where T : AggregateRoot
        {
            try
            {
                return _session.Get<T>(id, expectedVersion);
            }
            catch (Exception e)
            {
                logger.Error("Cannot get object of type {0} with id:{1} ({2}) from session", typeof(T), id, expectedVersion);
                throw e;
            }
        }

        public void Handle(ChangeOrderAddressCommand command)
        {
            logger.Info("Handle ChangeOrderAddressCommand {0} ({1})", command.Id, command.ExpectedVersion);
            Order item = Get<Order>(command.Id, command.ExpectedVersion);
            item.ChangeAddress(command.NewAddress);
            _session.Commit();
        }

        public void Handle(DeleteOrderCommand command)
        {
            Order item = Get<Order>(command.Id, command.ExpectedVersion);
            item.Delete();
            _session.Commit();
        }
    }
    }
