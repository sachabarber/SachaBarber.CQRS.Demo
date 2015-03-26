using System.ComponentModel;
using System.ServiceProcess;

namespace SachaBarber.CQRS.Demo.Orders.Domain.Host
{
    [RunInstaller(true)]
    public partial class OrderServiceDomainHostInstaller : System.Configuration.Install.Installer
    {
        public OrderServiceDomainHostInstaller()
        {
            InitializeComponent();
            Install();
        }

        public void Install()
        {
            Installers.Add(new ServiceInstaller
            {
                ServiceName = "SachaBarber CQRS Demo OrderService",
                DisplayName = "SachaBarber CQRS Demo OrderService",
                Description = "Provides orders API for CQRS demo",
                StartType = ServiceStartMode.Automatic
            });

            Installers.Add(new ServiceProcessInstaller {Account = ServiceAccount.LocalSystem});
        }
    }
}
