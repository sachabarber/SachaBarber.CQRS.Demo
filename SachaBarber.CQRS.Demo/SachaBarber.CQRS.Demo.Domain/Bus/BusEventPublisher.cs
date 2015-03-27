using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CQRSlite.Events;
using SachaBarber.CQRS.Demo.Orders.Domain.Events;
using SachaBarber.CQRS.Demo.Orders.Domain.Events.Handlers;
using SachaBarber.CQRS.Demo.SharedCore.Exceptions;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Bus
{
    public class BusEventPublisher : IEventPublisher
    {
        private readonly IBusEventHandler[] _handlers;
        private Dictionary<Type,MethodInfo> methodLookups = new Dictionary<Type, MethodInfo>(); 

        public BusEventPublisher(IBusEventHandler[] handlers)
        {
            _handlers = handlers;

            foreach (var handler in _handlers)
            {
                var meth = (from m in handler.GetType()
                     .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            let prms = m.GetParameters()
                            where prms.Count() == 1 && m.Name.Contains("Handle")
                            select new
                            {
                                EventType = prms.First().ParameterType,
                                Method = m
                            }).FirstOrDefault();
                if (meth != null)
                {
                    methodLookups.Add(meth.EventType, meth.Method);
                }

            }

        }

        public void Publish<T>(T @event) where T : IEvent
        {

            var theHandler = _handlers.Single(x => x.HandlerType == @event.GetType());

            //var meth = (from m in theHandler.GetType()
            //     .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            //            let prms = m.GetParameters()
            //            where prms.Count() == 1 && prms[0].ParameterType == @event.GetType()
            //            select m).FirstOrDefault();

            //if (meth == null)
            //    throw new BusinessLogicException(
            //        string.Format("Handler for {0} could not be found", @event.GetType().Name));

            //meth.Invoke(theHandler, new [] { (object)@event });

            methodLookups[@event.GetType()].Invoke(theHandler, new [] { (object)@event });
          
        }


        public static Func<object, T> CreateStaticCastStatement<T>()
        {
            ParameterExpression inObject = Expression.Parameter(typeof(object));
            Expression convertExpression = Expression.Convert(inObject, typeof(T));
            return Expression.Lambda<Func<object, T>>(convertExpression, inObject).Compile();
        }

    }
}
