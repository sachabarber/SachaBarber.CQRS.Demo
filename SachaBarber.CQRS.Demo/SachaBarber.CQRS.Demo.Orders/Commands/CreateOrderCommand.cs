﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using CQRSlite.Commands;

namespace SachaBarber.CQRS.Demo.Orders.Commands
{
    [DataContract]
    public class CreateOrderCommand : Command
    {
        [DataMember]
        public string OrderDescription { get; set; }
    }
}
