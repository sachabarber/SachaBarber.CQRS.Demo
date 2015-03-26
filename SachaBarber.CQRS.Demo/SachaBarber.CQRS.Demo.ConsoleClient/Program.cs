using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SachaBarber.CQRS.Demo.Orders;
using SachaBarber.CQRS.Demo.Orders.Commands;

namespace SachaBarber.CQRS.Demo.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {

            Guid id = Guid.NewGuid();


            IOrderService invoker = new OrderServiceInvoker();
            invoker.SendCommand(new CreateOrderCommand()
                                {
                                    ExpectedVersion = 1,
                                    Id = id,
                                    OrderDescription = "New Order A"
                                });


            invoker.SendCommand(new RenameOrderCommand()
            {
                ExpectedVersion = 1,
                Id = id,
                NewOrderDescription = "CHANGED New Order A"
            });



            Console.ReadLine();
        }
    }
}
