using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using SachaBarber.CQRS.Demo.WPFClient.Controls;
using SachaBarber.CQRS.Demo.WPFClient.ViewModels;

namespace SachaBarber.CQRS.Demo.WPFClient.Services
{
    public class DialogService : IDialogService
    {
        public bool? ShowDialog(IDialogViewModel viewModel)
        {
            DialogViewModelWindow window = new DialogViewModelWindow(viewModel);
            window.Owner = Application.Current.MainWindow;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return window.ShowDialog().GetValueOrDefault(false);
        }

       
    }
}
