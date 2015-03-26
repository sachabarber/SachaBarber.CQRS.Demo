using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.SharedCore.Faults
{
    [DataContract]
    public class GenericFault : IFault
    {
        [DataMember]
        public string Operation { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

    }
}
