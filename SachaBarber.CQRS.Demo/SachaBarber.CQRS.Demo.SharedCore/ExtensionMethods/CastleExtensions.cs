using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.MicroKernel;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Castle.Windsor;
using Castle.Windsor.Diagnostics;

using NLog;

using SachaBarber.CQRS.Demo.SharedCore.IOC;

namespace SachaBarber.CQRS.Demo.SharedCore.ExtensionMethods
{
    public static class CastleExtensions
    {

        private static Logger logger = LogManager.GetLogger("CastleExtensions");


        

        public static ComponentRegistration<T> ApplyLifeStyle<T>(this LifestyleGroup<T> component, ILifestyleApplier lifestyleApplier) where T : class
        {
            return lifestyleApplier.ApplyLifeStyle(component);
        }


        public static void CheckForPotentiallyMisconfiguredComponents(this IWindsorContainer container)
        {
            var host = (IDiagnosticsHost)container.Kernel.GetSubSystem(SubSystemConstants.DiagnosticsKey);
            var diagnostics = host.GetDiagnostic<IPotentiallyMisconfiguredComponentsDiagnostic>();

            var handlers = diagnostics.Inspect();
            if (!handlers.Any()) return;

            var message = new StringBuilder();
            var inspector = new DependencyInspector(message);

            foreach (IExposeDependencyInfo handler in handlers.Where(h => h.ComponentModel.Implementation.Assembly.FullName.StartsWith("Options.")))
            {
                handler.ObtainDependencyDetails(inspector);
            }

            if (message.Length > 0)
            {
                var ex = new System.Configuration.ConfigurationErrorsException(message.ToString());
                logger.Error("Castle Configuarion Error. Message : {0}\r\nStackTrace {1}", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

    }

}
