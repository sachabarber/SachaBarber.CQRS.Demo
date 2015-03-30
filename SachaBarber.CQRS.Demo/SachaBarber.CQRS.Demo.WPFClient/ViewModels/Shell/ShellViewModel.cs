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

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels.Shell
{
    public class ShellViewModel : AsyncDisposableViewModel
    {
        private readonly CreateOrderDialogViewModelFactory createOrderDialogViewModelFactory;
        private readonly IDialogService dialogService;
        private readonly IInterProcessBusSubscriber interProcessBusSubscriber;
        private object syncLock = new object();

        public ShellViewModel(
            CreateOrderDialogViewModelFactory createOrderDialogViewModelFactory,
            IDialogService dialogService,
            IInterProcessBusSubscriber interProcessBusSubscriber)
        {
            this.createOrderDialogViewModelFactory = createOrderDialogViewModelFactory;
            this.dialogService = dialogService;
            this.interProcessBusSubscriber = interProcessBusSubscriber;
            StoreItems = new ObservableCollection<StoreItemViewModel>();
            BindingOperations.EnableCollectionSynchronization(StoreItems, syncLock);

            CreateNewOrderCommand = new SimpleCommand<object, object>(
                CanExecuteCreateNewOrderCommand,
                ExecuteCreateNewOrderCommand);

            interProcessBusSubscriber.GetEventStream().Subscribe(async x =>
            {
                string mess = x;

                var orders = await new OrderServiceInvoker().CallService(service =>
                            service.GetAllOrders());

                var fgddsd = 56;
            });
        }


        public async Task Init()
        {
            this.WaitText = "Loading stored items from read model";
            this.AsyncState = AsyncType.Busy;
            await InitialiseReadModel();
            this.AsyncState = AsyncType.Content;
        }

        public Task<bool> InitialiseReadModel()
        {
            return Task.Run(
                async () =>
                {
                    var items = await new OrderServiceInvoker().CallService(service =>
                        service.GetAllStoreItems());

                    StoreItems.AddRange(items.Select(x => new StoreItemViewModel(x)));
                    return true;
                });
        }

        public ICommand CreateNewOrderCommand { get; private set; }

        public ObservableCollection<StoreItemViewModel> StoreItems { get; private set; }


        private bool CanExecuteCreateNewOrderCommand(object parameter)
        {
            return StoreItems.Any(x => x.IsSelected);
        }

        private async void ExecuteCreateNewOrderCommand(object parameter)
        {
            Guid orderId = Guid.NewGuid();

            var createOrderDialogViewModel =
                createOrderDialogViewModelFactory.Create(orderId, StoreItems.Where(x => x.IsSelected).ToList());

            var result = dialogService.ShowDialog(createOrderDialogViewModel);
            if (result.HasValue && result.Value)
            {
                var orderCreated = await new OrderServiceInvoker().CallService(service =>
                service.SendCommand(new CreateOrderCommand()
                {
                    ExpectedVersion = 1,
                    Id = orderId,
                    Address = "This is the address",
                    Description = "Description1",
                    OrderItems = createOrderDialogViewModel.OrderItems.ToList()
                }));

            }
        }

    }
}
