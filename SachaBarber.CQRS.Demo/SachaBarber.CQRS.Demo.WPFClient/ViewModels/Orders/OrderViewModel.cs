using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SachaBarber.CQRS.Demo.Orders;
using SachaBarber.CQRS.Demo.Orders.Commands;
using SachaBarber.CQRS.Demo.Orders.ReadModel.Models;
using SachaBarber.CQRS.Demo.WPFClient.Commands;
using SachaBarber.CQRS.Demo.WPFClient.Controls;
using SachaBarber.CQRS.Demo.WPFClient.Services;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels.Orders
{
    public class OrderViewModel : INPCBase
    {
        private readonly IMessageBoxService messageBoxService;
        private readonly OrderServiceInvoker orderServiceInvoker;
        private bool isEnabled = false;
        private string oldAddress = string.Empty;
        private bool operationInFlight = false;

        public OrderViewModel(
            Order order, 
            IMessageBoxService messageBoxService, 
            OrderServiceInvoker orderServiceInvoker)
        {
            this.messageBoxService = messageBoxService;
            this.orderServiceInvoker = orderServiceInvoker;
            this.Order = order;

            this.GotoEditModeCommand = new SimpleCommand<object, object>(
                x => !this.IsEnabled,
                ExecuteGotoEditModeCommand
            );

            this.DeleteOrderCommand = new SimpleCommand<object, object>(
                x => this.IsEnabled && !operationInFlight,
                ExecuteDeleteOrderCommand
            );

            this.CancelEditModeCommand = new SimpleCommand<object, object>(
                x => this.IsEnabled,
                ExecuteCancelEditModeCommand
            );

            this.ChangeOrderAddressCommand = new SimpleCommand<object, object>(
                x => this.IsEnabled && !operationInFlight,
                ExecuteChangeOrderAddressCommand
            );
        }

        public Order Order { get; private set; }
        public ICommand GotoEditModeCommand { get; private set; }
        public ICommand DeleteOrderCommand { get; private set; }
        public ICommand CancelEditModeCommand { get; private set; }
        public ICommand ChangeOrderAddressCommand { get; private set; }


        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            protected set
            {
                RaiseAndSetIfChanged(ref this.isEnabled, value, () => IsEnabled);
            }
        }

        private void ExecuteGotoEditModeCommand(object param)
        {
            this.oldAddress = this.Order.Address;
            this.IsEnabled = true;
            RaisePropertyChanged(()=> this.Order);
        }



        private async void ExecuteDeleteOrderCommand(object param)
        {
            try
            {
                operationInFlight = true;
                await orderServiceInvoker.CallService(service =>
                    service.SendCommandAsync(new DeleteOrderCommand()
                    {
                        ExpectedVersion = this.Order.Version,
                        Id = this.Order.OrderId
                    }));
            }
            catch (Exception e)
            {
                messageBoxService.ShowError("There was an error deleting the order");
            }
            finally
            {
                 operationInFlight = false;
            }
        }

        private void ExecuteCancelEditModeCommand(object param)
        {
            this.Order.Address = this.oldAddress;
            this.IsEnabled = false;
            RaisePropertyChanged(() => this.Order);
        }

        private async void ExecuteChangeOrderAddressCommand(object param)
        {
            try
            {
                operationInFlight = true;
                await orderServiceInvoker.CallService(service =>
                    service.SendCommandAsync(new ChangeOrderAddressCommand()
                    {
                        ExpectedVersion = this.Order.Version,
                        Id = this.Order.OrderId,
                        NewAddress = this.Order.Address
                    }));
            }
            catch (Exception e)
            {
                messageBoxService.ShowError("There was an error changing the address");
            }
            finally
            {
                 operationInFlight = false;
            }
        }

    }
}
