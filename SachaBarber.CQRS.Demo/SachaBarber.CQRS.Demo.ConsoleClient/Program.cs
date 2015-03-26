using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SachaBarber.CQRS.Demo.Orders;
using SachaBarber.CQRS.Demo.Orders.Commands;
using SachaBarber.CQRS.Demo.Orders.DTOs;

namespace SachaBarber.CQRS.Demo.ConsoleClient
{
    class Program
    {

        public async Task Run()
        {
            Guid id = Guid.NewGuid();



            var x = await new OrderServiceInvoker().CallService(service => 
                service.SendCommand(new CreateOrderCommand()
                {
                    ExpectedVersion = 1,
                    Id = id,
                    Address = "This is the address",
                    Description = "Description1",
                    OrderItems = new List<OrderItem>()
                }));
            

            //var x = await new OrderServiceClient().SendCommand(new CreateOrderCommand()
            //{
            //    ExpectedVersion = 1,
            //    Id = id,
            //    Address = "This is the address",
            //    Description = "Description1",
            //    OrderItems = new List<OrderItem>()
            //});


            var y = 54554;

            //invoker.SendCommand(new RenameOrderCommand()
            //{
            //    ExpectedVersion = 1,
            //    Id = id,
            //    NewOrderDescription = "CHANGED New Order A"
            //});

        }


        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run().Wait();
           


            Console.ReadLine();
        }
    }
}
