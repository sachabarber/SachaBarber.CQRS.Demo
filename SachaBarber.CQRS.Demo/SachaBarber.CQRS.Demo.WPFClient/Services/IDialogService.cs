using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SachaBarber.CQRS.Demo.WPFClient.ViewModels;

namespace SachaBarber.CQRS.Demo.WPFClient.Services
{
    public interface IDialogService
    {
        bool? ShowDialog(IDialogViewModel viewModel);
    }
}
