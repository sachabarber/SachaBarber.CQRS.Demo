using System;
using System.IO;
using System.Threading;
using NLog;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Host
{
    static class Program
    {

        private static Logger logger = LogManager.GetLogger("Program");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if (!DEBUG)
            try
            {
                logger.InfoFormat("Initialising SachaBarber.CQRS.Demo.OrderService in assembly {0} RELEASE windows service mode.", System.Reflection.Assembly.GetExecutingAssembly().FullName);

                System.ServiceProcess.ServiceBase.Run(new Service());
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
#else
            try
            {
                logger.Info("Initialising SachaBarber.CQRS.Demo.OrderService");
                OrderServiceRunner dealingServiceRunner = new OrderServiceRunner();
                dealingServiceRunner.Start();
                ManualResetEvent mre = new ManualResetEvent(false);
                Console.CancelKeyPress += (s, e) =>
                {
                    if (e.SpecialKey == ConsoleSpecialKey.ControlC)
                    {
                        mre.Set();
                    }
                };

                mre.WaitOne();
                dealingServiceRunner.Stop();
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
#endif
        }
    }
}
