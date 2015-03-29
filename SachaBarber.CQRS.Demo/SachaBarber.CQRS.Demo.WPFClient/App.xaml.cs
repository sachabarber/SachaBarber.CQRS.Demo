using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using NLog;

namespace SachaBarber.CQRS.Demo.WPFClient
{
    public partial class App : Application
    {
        private static Logger logger = LogManager.GetLogger("App");

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            logger.Info("Starting SachaBarber.CQRS.Demo.WPFClient");

            //show tooltip on disabled for all controls
            ToolTipService.ShowOnDisabledProperty.OverrideMetadata(typeof(Control),
                      new FrameworkPropertyMetadata(true));

            new Bootstrapper().Run();
        }
    }
}
