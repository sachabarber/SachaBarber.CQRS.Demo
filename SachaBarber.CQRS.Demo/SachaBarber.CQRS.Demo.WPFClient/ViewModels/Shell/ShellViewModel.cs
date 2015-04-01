using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using Raven.Abstractions.Extensions;

using SachaBarber.CQRS.Demo.Orders.ReadModel;
using SachaBarber.CQRS.Demo.Orders.ReadModel.Models;
using SachaBarber.CQRS.Demo.WPFClient.Controls;
using System.Windows.Input;
using SachaBarber.CQRS.Demo.Orders;
using SachaBarber.CQRS.Demo.Orders.Commands;
using SachaBarber.CQRS.Demo.WPFClient.Commands;
using SachaBarber.CQRS.Demo.WPFClient.Services;
using SachaBarber.CQRS.Demo.WPFClient.ViewModels.Orders;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels.Shell
{
    public class ShellViewModel : AsyncDisposableViewModel
    {
        private readonly CreateOrderDialogViewModelFactory createOrderDialogViewModelFactory;
        private readonly IDialogService dialogService;
        private readonly OrderServiceInvoker orderServiceInvoker;
        private readonly IMessageBoxService messageBoxService;
        private object syncLock = new object();
        private bool hasItems = false;

        public ShellViewModel(
            CreateOrderDialogViewModelFactory createOrderDialogViewModelFactory,
            IDialogService dialogService,
            OrderServiceInvoker orderServiceInvoker,
            IMessageBoxService messageBoxService,
            Func<OrdersViewModel> ordersViewModelFactory)
        {
            this.OrdersViewModel = ordersViewModelFactory();
            this.createOrderDialogViewModelFactory = createOrderDialogViewModelFactory;
            this.dialogService = dialogService;
            this.orderServiceInvoker = orderServiceInvoker;
            this.messageBoxService = messageBoxService;
            StoreItems = new ObservableCollection<StoreItemViewModel>();
            BindingOperations.EnableCollectionSynchronization(StoreItems, syncLock);

            CreateNewOrderCommand = new SimpleCommand<object, object>(
                CanExecuteCreateNewOrderCommand,
                ExecuteCreateNewOrderCommand);

           
        }





        public async Task Init()
        {
            this.WaitText = "Loading stored items from read model";
            this.AsyncState = AsyncType.Busy;
            await InitialiseReadModel();
            this.AsyncState = AsyncType.Content;
        }

        public bool HasItems
        {
            get
            {
                return this.hasItems;
            }
            protected set
            {
                RaiseAndSetIfChanged(ref this.hasItems, value, () => HasItems);
            }
        }

 
        public ICommand CreateNewOrderCommand { get; private set; }

        public ObservableCollection<StoreItemViewModel> StoreItems { get; private set; }
        public OrdersViewModel OrdersViewModel { get; private set; }

        private Task<bool> InitialiseReadModel()
        {
            return Task.Run(
                async () =>
                {
                    try
                    {
                        var items = await orderServiceInvoker.CallService(service =>
                                            service.GetAllStoreItems());
                        StoreItems.AddRange(items.Select(x => new StoreItemViewModel(x)));
                        HasItems = StoreItems.Any();
                        return true;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                });
        }

        private bool CanExecuteCreateNewOrderCommand(object parameter)
        {
            return StoreItems.Any(x => x.IsSelected) && this.AsyncState != AsyncType.Busy;
        }

        private async void ExecuteCreateNewOrderCommand(object parameter)
        {
            Guid orderId = Guid.NewGuid();

            var createOrderDialogViewModel =
                createOrderDialogViewModelFactory.Create(orderId, StoreItems.Where(x => x.IsSelected).ToList());

            var result = dialogService.ShowDialog(createOrderDialogViewModel);
            if (result.HasValue && result.Value)
            {
                try
                {
                    this.AsyncState = AsyncType.Busy;
                    this.WaitText = "Saving order";
 
                    var orderCreated = await orderServiceInvoker.CallService(service =>
                        service.SendCommand(new CreateOrderCommand()
                        {
                            ExpectedVersion = 1,
                            Id = orderId,
                            Address = createOrderDialogViewModel.Address,
                            Description = createOrderDialogViewModel.Description,
                            OrderItems = createOrderDialogViewModel.OrderItems.ToList()
                        }));

                    if (orderCreated)
                    {
                        this.AsyncState = AsyncType.Content;
                    }
                }
                catch (Exception e)
                {
                    messageBoxService.ShowError("There was an error creating the order");
                    this.ErrorMessage = "There was an error creating the order";
                    this.AsyncState = AsyncType.Error;
                }
                
            }
        }

    }
}
