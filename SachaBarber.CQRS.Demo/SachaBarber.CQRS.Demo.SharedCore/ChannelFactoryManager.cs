using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.SharedCore
{
    public class ChannelFactoryManager : IDisposable
    {
        private static Dictionary<Type, ChannelFactory> factories = new Dictionary<Type, ChannelFactory>();
        private static readonly object _syncRoot = new object();

        public virtual T CreateChannel<T>() where T : class
        {
            return CreateChannel<T>("*", null);
        }

        public virtual T CreateChannel<T>(string endpointConfigurationName) where T : class
        {
            return CreateChannel<T>(endpointConfigurationName, null);
        }


        //called via reflection in ServiceInvokerBase class
        public virtual T CreateChannel<T>(
            string endpointConfigurationName, string endpointAddress) where T : class
        {
            T local = GetFactory<T>(endpointConfigurationName, endpointAddress).CreateChannel();
            ((ICommunicationObject)local).Faulted += new WeakEventHandler<EventArgs>(ChannelFaulted).Handler;

            return local;
        }

        protected virtual ChannelFactory<T> GetFactory<T>(
            string endpointConfigurationName, string endpointAddress) where T : class
        {
            lock (_syncRoot)
            {
                ChannelFactory factory;
                if (!factories.TryGetValue(typeof(T), out factory))
                {
                    factory = CreateFactoryInstance<T>(endpointConfigurationName, endpointAddress);
                    factories.Add(typeof(T), factory);
                }
                else
                {
                    if (factory.State == CommunicationState.Faulted)
                    {
                        factories.Remove(typeof(T));
                        factory = CreateFactoryInstance<T>(endpointConfigurationName, endpointAddress);
                        factories.Add(typeof(T), factory);
                    }
                }
                return (factory as ChannelFactory<T>);
            }
        }

        private ChannelFactory CreateFactoryInstance<T>(
            string endpointConfigurationName, string endpointAddress)
        {
            ChannelFactory factory = null;
            if (!string.IsNullOrEmpty(endpointAddress))
            {
                factory = new ChannelFactory<T>(endpointConfigurationName, new EndpointAddress(endpointAddress));
            }
            else
            {
                factory = new ChannelFactory<T>(endpointConfigurationName);
            }
            factory.Faulted += new WeakEventHandler<EventArgs>(FactoryFaulted).Handler;
            factory.Open();
            return factory;
        }

        private void ChannelFaulted(object sender, EventArgs e)
        {
            ICommunicationObject channel = (ICommunicationObject)sender;
            try
            {
                channel.Close();
            }
            catch
            {
                channel.Abort();
            }
            RecreateFactoryOnFault<ICommunicationObject>(channel);
            throw new ApplicationException("Exc_ChannelFailure");
        }



        private void RecreateFactoryOnFault<T>(object commsObject)
        {
            Type[] genericArguments = typeof(T).GetGenericArguments();
            if ((genericArguments != null) && (genericArguments.Length == 1))
            {
                Type key = genericArguments[0];
                if (factories.ContainsKey(key))
                {
                    factories.Remove(key);
                }
            }
        }

        private void FactoryFaulted(object sender, EventArgs args)
        {
            ChannelFactory factory = (ChannelFactory)sender;
            try
            {
                factory.Close();
            }
            catch
            {
                factory.Abort();
            }
            RecreateFactoryOnFault<ChannelFactory>(factory);
            throw new ApplicationException("Exc_ChannelFactoryFailure");
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_syncRoot)
                {
                    foreach (Type type in factories.Keys)
                    {
                        ChannelFactory factory = factories[type];
                        try
                        {
                            factory.Close();
                        }
                        catch
                        {
                            factory.Abort();
                        }
                    }
                    factories.Clear();
                }
            }
        }
    }
}
