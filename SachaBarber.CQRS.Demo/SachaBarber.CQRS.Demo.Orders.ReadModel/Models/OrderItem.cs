using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.Orders.ReadModel.Models
{
    public class OrderItem
    {
        public string OrderItemId { get; set; }

        public Guid OrderId { get; set; }

        public Guid StoreItemId { get; set; }

        public string StoreItemDescription { get; set; }

        public string StoreItemUrl { get; set; }

    }
}
