using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SachaBarber.CQRS.Demo.Orders.ReadModel.Models;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels.Shell
{
    public class StoreItemViewModel
    {
        public StoreItemViewModel(StoreItem item)
        {
            this.Id = item.Id;
            this.Description = item.Description;
            this.ImageUrl = string.Format("/SachaBarber.CQRS.Demo.WPFClient;component/Images/{0}", item.ImageUrl);
        }

        public Guid Id { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
