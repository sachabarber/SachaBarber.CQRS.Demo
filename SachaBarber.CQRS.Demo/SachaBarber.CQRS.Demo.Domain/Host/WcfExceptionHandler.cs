using System;
using System.ServiceModel.Dispatcher;

using NLog;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Host
{
    public class WcfExceptionHandler : ExceptionHandler
    {
        private Logger logger = LogManager.GetLogger("WcfExceptionHandler");

        public override bool HandleException(Exception exception)
        {
            logger.Error("SachaBarber.CQRS.Demo.Orders.Domain.Host : {0}", exception.StackTrace);
            return true;
        }
    }
}
