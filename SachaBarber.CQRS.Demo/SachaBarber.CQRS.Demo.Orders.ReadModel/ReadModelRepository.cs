using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Embedded;

using SachaBarber.CQRS.Demo.Orders.ReadModel.Models;
using Raven.Client.Indexes;

namespace SachaBarber.CQRS.Demo.Orders.ReadModel
{


    public interface IReadModelRepository
    {
        Task<bool> CreateFreshDb();
        Task<bool> SeedProducts();
        Task<List<T>> GetAll<T>();
        Task<bool> AddOrder(Order order);
    }


    public class ReadModelRepository : IReadModelRepository
    {

        private readonly IDocumentStore documentStore = null;
        private string dataDir = @"C:\temp\RavenDb";

        public ReadModelRepository()
        {
            documentStore = new EmbeddableDocumentStore
            {
                DataDirectory = dataDir,
                UseEmbeddedHttpServer = true
            };
            documentStore.Initialize();


            //Add order Index
            if (documentStore.DatabaseCommands.GetIndex("Order/ById") == null)
            {
                documentStore.DatabaseCommands.PutIndex(
                    "Order/ById",
                    new IndexDefinitionBuilder<Order> { Map = ords => from order in ords select new { order.Id } });
            }

        }

        public async Task<bool> CreateFreshDb()
        {
            var storeItems = await this.GetAll<StoreItem>();
            if (!storeItems.Any())
            {
                await SeedProducts();
            }
            await DeleteAllOrders();
            return true;
        }

        public Task<bool> SeedProducts()
        {

            return Task.Run(() =>
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                         CreateStoreItem(session,"RatGood.jpg","Rat God");
                         CreateStoreItem(session, "NeverBoy.jpg", "Never Boy");
                         CreateStoreItem(session, "Witcher.jpg", "Witcher");
                         CreateStoreItem(session, "Eight.jpg", "Eight");
                         CreateStoreItem(session, "MisterX.jpg", "Mister X");
                         CreateStoreItem(session, "CaptainMidnight.jpg", "Captain Midnight");
                        session.SaveChanges();
                    }
                    return true;
                });



        }



        private void CreateStoreItem(IDocumentSession session, string imageUrl, string description)
        {
            StoreItem newStoreItem = new StoreItem
            {
                Id = Guid.NewGuid(),
                ImageUrl = imageUrl,
                Description = description
            };
            session.Store(newStoreItem);
        }



        public Task<List<T>> GetAll<T>()
        {
            List<T> items = new List<T>();

            return Task.Run(() =>
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        int start = 0;
                        while (true)
                        {
                            var current = session.Query<T>().Take(1024).Skip(start).ToList();
                            if (current.Count == 0) break;

                            start += current.Count;
                            items.AddRange(current);

                        }
                    }
                    return items;
                });
        }

        public Task<bool> AddOrder(Order order)
        {
            return Task.Run(() =>
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        session.Store(order);
                        session.SaveChanges();
                    }
                    return true;
                });
        }


        private async Task<bool> DeleteAllOrders()
        {
            var staleIndexesWaitAction = new Action(
                delegate
                {
                    while (documentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length != 0)
                    {
                        Thread.Sleep(10);
                    }
                });
            staleIndexesWaitAction.Invoke();
            documentStore.DatabaseCommands.DeleteByIndex("Order/ById", new IndexQuery());
            staleIndexesWaitAction.Invoke();
            return true;
        }
    }
}
