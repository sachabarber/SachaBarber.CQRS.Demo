using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.SharedCore.Faults
{
    public interface IFault
    {
        string Operation { get; set; }
        string Message { get; set; }
        string StackTrace { get; set; }
    }
}
