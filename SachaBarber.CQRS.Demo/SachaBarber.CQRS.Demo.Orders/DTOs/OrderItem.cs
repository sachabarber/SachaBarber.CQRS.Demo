using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.Orders.DTOs
{
    [DataContract]
    public class OrderItem
    {
        [DataMember]
        public Guid OrderId { get; set; }

        [DataMember]
        public Guid StoreItemId { get; set; }

        [DataMember]
        public string StoreItemDescription { get; set; }
        
 
    }
}
