using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SachaBarber.CQRS.Demo.WPFClient
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //show tooltip on disabled for all controls
            ToolTipService.ShowOnDisabledProperty.OverrideMetadata(typeof(Control),
                      new FrameworkPropertyMetadata(true));

            new Bootstrapper().Run();
        }
    }
}
