using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.WPFClient.Commands
{
    public interface IReactiveCommand
    {
        IObservable<object> CommandExecutedStream { get; }
    }
}
