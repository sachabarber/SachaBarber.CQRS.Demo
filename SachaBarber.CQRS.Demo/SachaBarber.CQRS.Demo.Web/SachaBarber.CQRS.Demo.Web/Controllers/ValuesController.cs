using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SachaBarber.CQRS.Demo.Orders;
using SachaBarber.CQRS.Demo.Orders.Commands;
using SachaBarber.CQRS.Demo.Orders.ReadModel.Models;

namespace SachaBarber.CQRS.Demo.Web.Controllers
{
    [Authorize]
    public class OrdersController : ApiController
    {
        private readonly OrderServiceInvoker orderServiceInvoker;

        /// <summary>
        /// Gets all orders
        /// </summary>
        /// <returns></returns>
        // GET api/orders
        public IEnumerable<Order> Get()
        {
            return new List<Order>();
        }

        // GET api/orders/guid
        public Order Get(Guid id)
        {
            return null;
        }

        /// <summary>
        /// Creates an order
        /// </summary>
        /// <param name="order"></param>
        // POST api/orders
        public async void Post([FromBody]Order order)
        {
            Guid orderId = Guid.NewGuid();

            await orderServiceInvoker.CallService(
                service =>
                    service.SendCommandAsync(new CreateOrderCommand()
                    {
                        ExpectedVersion = 1,
                        Id = orderId,
                        Address = order.Address,
                        Description = order.Description,
                        OrderItems = order.OrderItems.Select(o => new Orders.DTOs.OrderItem
                        {
                            StoreItemDescription = o.StoreItemDescription,
                            StoreItemId = o.StoreItemId,
                            StoreItemUrl = o.StoreItemUrl
                        }).ToList()
                    }));
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/orders/guid
        public async void Delete(Order order)
        {
            await orderServiceInvoker.CallService(
                service =>
                    service.SendCommandAsync(new DeleteOrderCommand()
                    {
                        ExpectedVersion = order.Version,
                        Id = order.OrderId
                    }));
        }
    }
}
