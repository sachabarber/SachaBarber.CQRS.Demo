using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.SharedCore.WCF
{
    public class ErrorHandlerBehaviorExtension : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new ErrorHandlerBehavior();
        }

        public override Type BehaviorType
        {
            get { return typeof(ErrorHandlerBehavior); }
        }
    }
}
