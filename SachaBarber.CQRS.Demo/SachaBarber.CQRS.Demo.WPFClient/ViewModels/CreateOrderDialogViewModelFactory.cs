using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SachaBarber.CQRS.Demo.WPFClient.Services;
using SachaBarber.CQRS.Demo.WPFClient.ViewModels.Shell;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels
{
    public class CreateOrderDialogViewModelFactory
    {
        private readonly IMessageBoxService messageBoxService;


        public CreateOrderDialogViewModelFactory(IMessageBoxService messageBoxService)
        {
            this.messageBoxService = messageBoxService;
        }

        public CreateOrderDialogViewModel Create(Guid orderId, IEnumerable<StoreItemViewModel> items)
        {
            return new CreateOrderDialogViewModel(messageBoxService, orderId, items);
        }
    }
}
