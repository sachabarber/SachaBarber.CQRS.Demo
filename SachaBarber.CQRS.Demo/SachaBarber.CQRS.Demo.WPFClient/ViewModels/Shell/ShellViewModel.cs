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
using SachaBarber.CQRS.Demo.WPFClient.Commands;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels.Shell
{
    public class ShellViewModel : AsyncDisposableViewModel
    {
        private readonly IReadModelRepository readModelRepository;
        private object syncLock = new object();

        public ShellViewModel(IReadModelRepository readModelRepository)
        {
            this.readModelRepository = readModelRepository;
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

        public Task<bool> InitialiseReadModel()
        {
            return Task.Run(
                async () =>
                {
                    await readModelRepository.CreateFreshDb();
                    var items = await readModelRepository.GetAll<StoreItem>();
                    StoreItems.AddRange(items.Select(x=> new StoreItemViewModel(x)));
                    return true;

                });
        }

        public ICommand CreateNewOrderCommand { get; private set; }

        public ObservableCollection<StoreItemViewModel> StoreItems { get; private set; }


        private bool CanExecuteCreateNewOrderCommand(object parameter)
        {
            return StoreItems.Any(x => x.IsSelected);
        }

        private void ExecuteCreateNewOrderCommand(object parameter)
        {
           //TODO
           //TODO
           //TODO
           //TODO
           //TODO
        }

    }
}
