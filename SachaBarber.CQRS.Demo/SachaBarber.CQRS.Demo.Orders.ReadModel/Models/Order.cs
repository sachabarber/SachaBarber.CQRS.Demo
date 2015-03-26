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
        public string OrderDescription { get; set; }
    }
}
