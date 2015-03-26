using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.Orders.ReadModel.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        public string Description { get; set; }
        
        public string Address { get; set; }

        public List<OrderItem> OrderItems { get; set; }

    }
}
