using CQRSlite.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.Orders.Commands
{
    [DataContract]
    [KnownType(typeof(CreateOrderCommand))]
    [KnownType(typeof(ChangeOrderAddressCommand))]
    public abstract class Command : ICommand
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public int ExpectedVersion { get; set; }
    }
}
