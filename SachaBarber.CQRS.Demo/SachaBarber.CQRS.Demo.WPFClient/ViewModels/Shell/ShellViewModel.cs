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

        public ObservableCollection<StoreItemViewModel> StoreItems { get; private set; }

    }
}
