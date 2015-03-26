using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.SharedCore.Faults
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class StandardFaultsAttribute : Attribute, IContractBehavior
    {
        // this is a list of our standard fault detail classes.
        static Type[] Faults = new Type[]
        {
            typeof(BusinessLogicFault),
            typeof(GenericFault)
        };

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
            ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription,
            ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
            foreach (OperationDescription op in contractDescription.Operations)
            {
                foreach (Type fault in Faults)
                {
                    op.Faults.Add(MakeFault(fault));
                }
            }
        }

        private FaultDescription MakeFault(Type detailType)
        {
            string action = detailType.Name;

            DescriptionAttribute description = (DescriptionAttribute)
                Attribute.GetCustomAttribute(detailType, typeof(DescriptionAttribute));

            if (description != null)
                action = description.Description;

            FaultDescription fd = new FaultDescription(action);
            fd.DetailType = detailType;
            fd.Name = detailType.Name;
            return fd;
        }
    }
}
