using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using SachaBarber.CQRS.Demo.Orders;
using SachaBarber.CQRS.Demo.Orders.ReadModel.Models;
using SachaBarber.CQRS.Demo.WPFClient.Services;
using System.Windows.Input;
using SachaBarber.CQRS.Demo.WPFClient.Commands;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels.Orders
{
    public class OrdersViewModel : INPCBase, IDisposable
    {
        private bool hasOrders = false;
        private List<OrderViewModel> orders = new List<OrderViewModel>();
        private CompositeDisposable disposables = new CompositeDisposable();
        private const double topOffset = 20;
        private const double leftOffset = 380;
        private readonly GrowlNotifications growlNotifications = new GrowlNotifications();
        private List<string> orderEvents;
        

        public OrdersViewModel(
            IInterProcessBusSubscriber interProcessBusSubscriber,
            OrderServiceInvoker orderServiceInvoker,
            IMessageBoxService messageBoxService)
        {
            orderEvents = new List<string>()
            {
                "OrderCreatedEvent","OrderAddressChangedEvent","OrderDeletedEvent"
            };

            growlNotifications.Top = SystemParameters.WorkArea.Top + topOffset;
            growlNotifications.Left = SystemParameters.WorkArea.Left + 
                SystemParameters.WorkArea.Width - leftOffset;

            var stream = interProcessBusSubscriber.GetEventStream();


            disposables.Add(stream.Where(x => orderEvents.Contains(x))
                .Subscribe(async x =>
                {
                    var newOrders = await orderServiceInvoker.CallService(service =>
                                service.GetAllOrdersAsync());

                    this.Orders = new List<OrderViewModel>(
                        newOrders.Select(ord => new OrderViewModel(ord, messageBoxService, orderServiceInvoker)));
                    this.HasOrders = Orders.Any();

                    if (this.HasOrders)
                    {
                        growlNotifications.AddNotification(new Notification
                        {
                            Title = "Orders changed",
                            ImageUrl = "pack://application:,,,/Images/metroInfo.png",
                            Message =
                                "New/modified orders have been obtained from the ReadModel. Click on the right hand side panel to see them"
                        });
                    }
                })
          );
           
        }

        public void Close()
        {
            growlNotifications.Close();
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

    
        public List<OrderViewModel> Orders
        {
            get
            {
                return this.orders;
            }
            protected set
            {
                RaiseAndSetIfChanged(ref this.orders, value, () => Orders);
            }
        }


        public void Dispose()
        {
            this.disposables.Dispose();
        }
    }
}
