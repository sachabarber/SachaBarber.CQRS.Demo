using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.Practices.Unity;

using SachaBarber.CQRS.Demo.WPFClient.ViewModels.Shell;

namespace SachaBarber.CQRS.Demo.WPFClient
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : Window
    {
        private Storyboard sbEaseInAnimation;
        private Storyboard sbEaseOutAnimation;

        private ShellViewModel viewModel;
        public ShellWindow
            (
                [Dependency]ShellViewModel viewModel
            )
        {
            this.viewModel = viewModel;
            this.DataContext = this.viewModel;
            InitializeComponent();
            this.Loaded += ShellWindow_Loaded;
        }

        async void ShellWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sbEaseInAnimation = this.FindResource("EaseInAnimation") as Storyboard;
            sbEaseOutAnimation = this.FindResource("EaseOutAnimation") as Storyboard;
            btn.Click += btn_Click;


            //Gives the service layer time to initialise the RavenDB
            await Task.Delay(10000); 

            await viewModel.Init();
        }

        private void btn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (btn.IsChecked == true)
            {
                sbEaseInAnimation.Begin();
            }
            else
            {
                sbEaseOutAnimation.Begin();
            }
        }
    }
}
