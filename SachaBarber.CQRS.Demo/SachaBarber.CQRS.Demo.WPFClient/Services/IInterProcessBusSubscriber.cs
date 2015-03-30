using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.WPFClient.Services
{
    public interface IInterProcessBusSubscriber
    {
        IObservable<string> GetEventStream();
    }
}
