using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SachaBarber.CQRS.Demo.Orders.ReadModel.Models;
using SachaBarber.CQRS.Demo.WPFClient.Commands;
using SachaBarber.CQRS.Demo.WPFClient.Services;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels.Orders
{
    public class OrderViewModel : INPCBase
    {
        private readonly IMessageBoxService messageBoxService;
        private bool isEnabled = false;

        public OrderViewModel(Order order, IMessageBoxService messageBoxService)
        {
            this.messageBoxService = messageBoxService;
            this.Order = order;

            this.GotoEditModeCommand = new SimpleCommand<object, object>(
                x => !this.IsEnabled,
                x => this.IsEnabled = true
            );

            this.CancelEditModeCommand = new SimpleCommand<object, object>(
                x => this.IsEnabled,
                x => this.IsEnabled = false
            );

            this.RenameOrderCommand = new SimpleCommand<object, object>(
                x => this.IsEnabled,
                x => messageBoxService.ShowError("TODO")
            );
        }

        public Order Order { get; private set; }

        public ICommand GotoEditModeCommand { get; private set; }
        public ICommand CancelEditModeCommand { get; private set; }
        public ICommand RenameOrderCommand { get; private set; }


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

    }
}
