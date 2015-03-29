using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.UnityExtensions;

using SachaBarber.CQRS.Demo.Orders.ReadModel;
using SachaBarber.CQRS.Demo.WPFClient.Services;
using SachaBarber.CQRS.Demo.WPFClient.ViewModels;

namespace SachaBarber.CQRS.Demo.WPFClient
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IReadModelRepository, ReadModelRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IMessageBoxService, MessageBoxService>(new TransientLifetimeManager());
            Container.RegisterType<IDialogService, DialogService>(new TransientLifetimeManager());
            Container.RegisterType<ISchedulerService, SchedulerService>(new TransientLifetimeManager());
            Container.RegisterType<CreateOrderDialogViewModelFactory>(new ContainerControlledLifetimeManager());
        }

        protected override DependencyObject CreateShell()
        {
            return this.Container.Resolve<ShellWindow>();

        }


        protected override void InitializeShell()
        {

          
            base.InitializeShell();
            Window shellWindow = (Window)Shell;


           

            App.Current.MainWindow = shellWindow;


            App.Current.MainWindow.Show();
            shellWindow.WindowState = WindowState.Maximized;
            shellWindow.Activate();

        }
    }
}
