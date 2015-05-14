using System;
using System.Collections.Generic;
using System.IO;
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
        Task<bool> DeleteOrder(Guid orderId);
        Task<bool> UpdateOrderAddress(Guid orderId, string newAddress, int version);
        Task<Order> GetOrder(Guid orderId);
    }


    public class ReadModelRepository : IReadModelRepository
    {

        private IDocumentStore documentStore = null;
        private string dataDir = @"C:\temp\RavenDb";

        public ReadModelRepository()
        {
       
        }

        public async Task<bool> CreateFreshDb()
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
                    new IndexDefinitionBuilder<Order>
                    {
                        Map = ords => from order in ords 
                                        select new { Id = order.Id }
                    });
            }

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
                            var current = session.Query<T>()
                                .Customize(x => x.WaitForNonStaleResults())
                                .Take(1024).Skip(start).ToList();
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

        public Task<bool> DeleteOrder(Guid orderId)
        {
            return Task.Run(() =>
            {
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var order = session.Query<Order>()
                        .SingleOrDefault(x => x.OrderId == orderId);
                    session.Delete(order);
                    session.SaveChanges();
                }
                return true;
            });
        }

        public Task<bool> UpdateOrderAddress(Guid orderId, string newAddress, int version)
        {
            return Task.Run(() =>
            {
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var order = session.Query<Order>()
                        .SingleOrDefault(x => x.OrderId == orderId);
                    order.Address = newAddress;
                    order.Version = version;
                    session.SaveChanges();
                }
                return true;
            });
        }


        public Task<Order> GetOrder(Guid orderId)
        {
            return Task.Run(() =>
            {
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    return session.Query<Order>()
                        .SingleOrDefault(x => x.OrderId == orderId);
                }
            });
        }




        private void CreateStoreItem(IDocumentSession session, string imageUrl, 
            string description)
        {
            StoreItem newStoreItem = new StoreItem
            {
                StoreItemId = Guid.NewGuid(),
                ImageUrl = imageUrl,
                Description = description
            };
            session.Store(newStoreItem);
        }

        private async Task<bool> DeleteAllOrders()
        {
            await Task.Run(() =>
            {
                var staleIndexesWaitAction = new Action(() =>
                    {
                        while (documentStore.DatabaseCommands.GetStatistics()
                            .StaleIndexes.Length != 0)
                        {
                            Thread.Sleep(10);
                        }
                    });
                staleIndexesWaitAction.Invoke();
                documentStore.DatabaseCommands
                    .DeleteByIndex("Order/ById", new IndexQuery());
                staleIndexesWaitAction.Invoke();
            });
            return true;
        }
    }
}
