using System;

namespace SachaBarber.CQRS.Demo.SharedCore.Services
{
    public interface IInterProcessBusSubscriber
    {
        IObservable<string> GetEventStream();
    }
}
