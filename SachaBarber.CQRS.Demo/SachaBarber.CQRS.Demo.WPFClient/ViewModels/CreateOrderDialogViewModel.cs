using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SachaBarber.CQRS.Demo.WPFClient.Commands;
using SachaBarber.CQRS.Demo.WPFClient.ViewModels.Shell;
using System.Windows.Input;
using SachaBarber.CQRS.Demo.Orders.DTOs;
using SachaBarber.CQRS.Demo.WPFClient.Services;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels
{
    public class CreateOrderDialogViewModel : DialogViewModel
    {
        private readonly IMessageBoxService messageBoxService;
        private string address;
        private string description;

        public CreateOrderDialogViewModel(
            IMessageBoxService messageBoxService,
            Guid orderId,
            IEnumerable<StoreItemViewModel> items) : base("Create Order")
        {
            this.messageBoxService = messageBoxService;
            this.OrderItems = items.Select(x => new OrderItem()
            {
                OrderId = orderId,
                StoreItemId = x.Id,
                StoreItemDescription = x.Description,
                StoreItemUrl = x.ImageUrl
            });
            OkCommand = new SimpleCommand<object, object>(x => Validate());
        }

        public ICommand OkCommand { get; private set; }     
        public IEnumerable<OrderItem> OrderItems { get; private set; }     



        public string Address
        {
            get { return address; }
            set
            {
                RaiseAndSetIfChanged(ref address, value, () => Address);
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                RaiseAndSetIfChanged(ref description, value, () => Description);
            }
        }

        private void Validate()
        {
            if (!string.IsNullOrEmpty(this.Address) && !string.IsNullOrEmpty(this.Description))
            {
                this.CloseDialog(true);
            }
            else
            {
                messageBoxService.ShowError("Invalid form, please fill in order fully");
            }
        }
    }
}
