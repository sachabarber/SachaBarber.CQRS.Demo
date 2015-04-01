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
using System.Windows.Input;
using SachaBarber.CQRS.Demo.WPFClient.Commands;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels.Orders
{
    public class OrdersViewModel : INPCBase
    {
        private readonly IMessageBoxService messageBoxService;
        private bool hasOrders = false;
        private List<OrderViewModel> orders = new List<OrderViewModel>();
        

        public OrdersViewModel(
            IInterProcessBusSubscriber interProcessBusSubscriber,
            OrderServiceInvoker orderServiceInvoker,
            IMessageBoxService messageBoxService)
        {
            this.messageBoxService = messageBoxService;

            interProcessBusSubscriber.GetEventStream().Subscribe(async x =>
            {
                var newOrders = await orderServiceInvoker.CallService(service =>
                            service.GetAllOrders());

                this.Orders = new List<OrderViewModel>(
                    newOrders.Select(ord => new OrderViewModel(ord, messageBoxService)));
                this.HasOrders = Orders.Any();

                messageBoxService.ShowInformation("New orders available, click the right hand side button to reveal them");

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

    
    }
}
