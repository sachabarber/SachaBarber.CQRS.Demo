using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.WPFClient.ExtensionMethods
{
    public class ItemPropertyChangedEvent<TSender>
    {
        public TSender Sender { get; set; }
        public PropertyInfo Property { get; set; }
        public bool HasOld { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }

        public override string ToString()
        {
            return string.Format("Sender: {0}, Property: {1}, HasOld: {2}, OldValue: {3}, NewValue: {4}", this.Sender, this.Property, this.HasOld, this.OldValue, this.NewValue);
        }
    }

    public class ItemPropertyChangedEvent<TSender, TProperty>
    {
        public TSender Sender { get; set; }
        public PropertyInfo Property { get; set; }
        public bool HasOld { get; set; }
        public TProperty OldValue { get; set; }
        public TProperty NewValue { get; set; }
    }

    public class ItemChanged<T>
    {
        public T Item { get; set; }
        public bool Added { get; set; }
        public NotifyCollectionChangedEventArgs EventArgs { get; set; }
    }


    public class WeakSubscription<T> : IDisposable, IObserver<T>
    {
        private readonly WeakReference reference;
        private readonly IDisposable subscription;
        private bool disposed;

        public WeakSubscription(IObservable<T> observable, IObserver<T> observer)
        {
            this.reference = new WeakReference(observer);
            this.subscription = observable.Subscribe(this);
        }

        void IObserver<T>.OnCompleted()
        {
            var observer = (IObserver<T>)this.reference.Target;
            if (observer != null)
                observer.OnCompleted();
            else
                this.Dispose();
        }


        void IObserver<T>.OnError(Exception error)
        {
            var observer = (IObserver<T>)this.reference.Target;
            if (observer != null)
                observer.OnError(error);
            else
                this.Dispose();
        }

        void IObserver<T>.OnNext(T value)
        {
            var observer = (IObserver<T>)this.reference.Target;
            if (observer != null)
                observer.OnNext(value);
            else
                this.Dispose();
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                this.subscription.Dispose();
            }
        }
    }

    public static class ObservableExtensions
    {
        public static IObservable<Unit> AsUnit<TValue>(this IObservable<TValue> source)
        {
            return source.Select(x => new Unit());
        }

        public static IObservable<TItem> ObserveWeakly<TItem>(this IObservable<TItem> source)
        {
            return Observable.Create<TItem>(obs =>
            {
                var weakSubscription = new WeakSubscription<TItem>(source, obs);
                return () =>
                {
                    weakSubscription.Dispose();
                };
            });
        }


        public static IObservable<Unit> ObserveCollectonChanged<T>(this T source)
           where T : INotifyCollectionChanged
        {
            var observable = Observable
                .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    h => source.CollectionChanged += h,
                    h => source.CollectionChanged -= h)
                .AsUnit();

            return observable;
        }


        public static IObservable<Unit> ObserveCollectonChanged<T>(this T source, NotifyCollectionChangedAction collectionChangeAction)
           where T : INotifyCollectionChanged
        {
            var observable = Observable
                .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    h => source.CollectionChanged += h,
                    h => source.CollectionChanged -= h)
                .Where(x => x.Action == collectionChangeAction)
                .AsUnit();

            return observable;
        }

        public static IObservable<ItemChanged<T>> ItemChanged<T>(this ObservableCollection<T> collection, bool fireForExisting = false)
        {
            var observable = Observable.Create<ItemChanged<T>>(obs =>
            {
                NotifyCollectionChangedEventHandler handler = null;
                handler = (s, a) =>
                {
                    if (a.NewItems != null)
                    {
                        foreach (var item in a.NewItems.OfType<T>())
                        {
                            obs.OnNext(new ItemChanged<T>()
                            {
                                Item = item,
                                Added = true,
                                EventArgs = a
                            });
                        }
                    }
                    if (a.OldItems != null)
                    {
                        foreach (var item in a.OldItems.OfType<T>())
                        {
                            obs.OnNext(new ItemChanged<T>()
                            {
                                Item = item,
                                Added = false,
                                EventArgs = a
                            });
                        }
                    }
                };
                collection.CollectionChanged += handler;
                return () =>
                {
                    collection.CollectionChanged -= handler;
                };
            });

            if (fireForExisting)
                observable = observable.StartWith(Scheduler.CurrentThread, collection.Select(i => new ItemChanged<T>()
                {
                    Item = i,
                    Added = true,
                    EventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, i)
                }).ToArray());

            return observable;
        }


        public static IObservable<TObserved> ObserveInner<TItem, TObserved>(this ObservableCollection<TItem> collection, Func<TItem, IObservable<TObserved>> observe)
        {
            return Observable.Create<TObserved>(obs =>
            {
                Dictionary<TItem, IDisposable> subscriptions = new Dictionary<TItem, IDisposable>();

                var mainSubscription =
                    collection.ItemChanged(true)
                        .Subscribe(change =>
                        {
                            IDisposable subscription = null;
                            subscriptions.TryGetValue(change.Item, out subscription);
                            if (change.Added)
                            {
                                if (subscription == null)
                                {
                                    subscription = observe(change.Item).Subscribe(obs);
                                    subscriptions.Add(change.Item, subscription);
                                }
                            }
                            else
                            {
                                if (subscription != null)
                                {
                                    subscriptions.Remove(change.Item);
                                    subscription.Dispose();
                                }
                            }
                        });

                return () =>
                {
                    mainSubscription.Dispose();
                    foreach (var subscription in subscriptions)
                        subscription.Value.Dispose();
                };
            });

        }

        public static IObservable<TValue> ObserveProperty<T, TValue>(this T source,
            Expression<Func<T, TValue>> propertyExpression) where T : INotifyPropertyChanged
        {
            return source.ObserveProperty(propertyExpression, false);
        }

        public static IObservable<TValue> ObserveProperty<T, TValue>(this T source,
            Expression<Func<T, TValue>> propertyExpression,
            bool observeInitialValue) where T : INotifyPropertyChanged
        {
            var getter = propertyExpression.Compile();

            var observable = Observable
                .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => source.PropertyChanged += h,
                    h => source.PropertyChanged -= h)
                .Where(x => x.PropertyName == propertyExpression.GetPropertyName())
                .Select(_ => getter(source));

            if (observeInitialValue)
                return observable.Merge(Observable.Return(getter(source)));

            return observable;
        }


        public static IObservable<string> ObservePropertyChanged<T>(this T source)
           where T : INotifyPropertyChanged
        {
            var observable = Observable
                .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => source.PropertyChanged += h,
                    h => source.PropertyChanged -= h)
                .Select(x => x.PropertyName);

            return observable;
        }


        public static IObservable<ItemPropertyChangedEvent<TItem, TProperty>> ObservePropertyChanged<TItem, TProperty>(this TItem target, Expression<Func<TItem, TProperty>> propertyName, bool fireCurrentValue = false) where TItem : INotifyPropertyChanged
        {
            var property = ExpressionExtensions.GetPropertyName(propertyName);

            return ObservePropertyChanged(target, property, fireCurrentValue)
                   .Select(i => new ItemPropertyChangedEvent<TItem, TProperty>()
                   {
                       HasOld = i.HasOld,
                       NewValue = (TProperty)i.NewValue,
                       OldValue = i.OldValue == null ? default(TProperty) : (TProperty)i.OldValue,
                       Property = i.Property,
                       Sender = i.Sender
                   });
        }


        public static IObservable<ItemPropertyChangedEvent<TItem>> ObservePropertyChanged<TItem>(this TItem target, string propertyName = null, bool fireCurrentValue = false) where TItem : INotifyPropertyChanged
        {
            if (propertyName == null && fireCurrentValue)
                throw new InvalidOperationException("You need to specify a propertyName if you want to fire the current value of your property");

            return Observable.Create<ItemPropertyChangedEvent<TItem>>(obs =>
            {
                Dictionary<PropertyInfo, object> oldValues = new Dictionary<PropertyInfo, object>();
                Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
                PropertyChangedEventHandler handler = null;

                handler = (s, a) =>
                {
                    if (propertyName == null || propertyName == a.PropertyName)
                    {
                        PropertyInfo prop = null;
                        if (!properties.TryGetValue(a.PropertyName, out prop))
                        {
                            prop = typeof(TItem).GetProperty(a.PropertyName);
                            properties.Add(a.PropertyName, prop);
                        }
                        var change = new ItemPropertyChangedEvent<TItem>()
                        {
                            Sender = target,
                            Property = prop,
                            NewValue = prop.GetValue(target, null)
                        };
                        object oldValue = null;
                        if (oldValues.TryGetValue(prop, out oldValue))
                        {
                            change.HasOld = true;
                            change.OldValue = oldValue;
                            oldValues[prop] = change.NewValue;
                        }
                        else
                        {
                            oldValues.Add(prop, change.NewValue);
                        }
                        obs.OnNext(change);
                    }
                };

                target.PropertyChanged += handler;

                if (propertyName != null && fireCurrentValue)
                    handler(target, new PropertyChangedEventArgs(propertyName));

                return () =>
                {
                    target.PropertyChanged -= handler;
                };
            });
        }












    }
}
