using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using CQRSlite.Commands;
using SachaBarber.CQRS.Demo.Orders.DTOs;

namespace SachaBarber.CQRS.Demo.Orders.Commands
{
    [DataContract]
    public class DeleteOrderCommand : Command
    {
      

    }
}
