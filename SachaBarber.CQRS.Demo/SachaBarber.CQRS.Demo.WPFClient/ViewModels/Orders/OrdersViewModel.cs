using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using SachaBarber.CQRS.Demo.Orders;
using SachaBarber.CQRS.Demo.Orders.ReadModel.Models;
using SachaBarber.CQRS.Demo.WPFClient.Services;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels.Orders
{
    public class OrdersViewModel : INPCBase
    {
        private object syncLock = new object();
        private bool hasOrders = false;

        public OrdersViewModel(
            IInterProcessBusSubscriber interProcessBusSubscriber,
            OrderServiceInvoker orderServiceInvoker)
        {
            Orders = new ObservableCollection<Order>();

            BindingOperations.EnableCollectionSynchronization(Orders, syncLock);

            interProcessBusSubscriber.GetEventStream().Subscribe(async x =>
            {
                var newOrders = await orderServiceInvoker.CallService(service =>
                            service.GetAllOrders());

                this.Orders = new ObservableCollection<Order>(newOrders);
                this.HasOrders = Orders.Any();
            });
        }

        public bool HasOrders
        {
            get
            {
                return this.hasOrders;
            }
            protected set
            {
                RaiseAndSetIfChanged(ref this.hasOrders, value, () => HasOrders);
            }
        }


        public ObservableCollection<Order> Orders { get; private set; }
    }
}
