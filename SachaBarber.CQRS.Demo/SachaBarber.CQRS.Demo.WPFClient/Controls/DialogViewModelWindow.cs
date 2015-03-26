using SachaBarber.CQRS.Demo.WPFClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace SachaBarber.CQRS.Demo.WPFClient.Controls
{
    public class DialogViewModelWindow : Window
    {
        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;
        private const uint SC_CLOSE = 0xF060;
        private const int WM_SHOWWINDOW = 0x00000018;

        public IDialogViewModel ViewModel
        {
            get;
            private set;
        }

        public DialogViewModelWindow(IDialogViewModel dialogViewModel)
        {
            this.ViewModel = dialogViewModel;
            Content = dialogViewModel;
            ContentRendered += DialogViewModelWindow_ContentRendered;

            if (!dialogViewModel.IsClosable)
            {
                SourceInitialized += DialogViewModelWindow_SourceInitialized;
                this.KeyDown += DialogViewModelWindow_KeyDown;
            }

            this.Loaded += DialogViewModelWindow_Loaded;

        }

        void DialogViewModelWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel is IDialogViewModelWithWindowEvents)
            {
                (ViewModel as IDialogViewModelWithWindowEvents).WindowLoaded();
            }
        }

        static DialogViewModelWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogViewModelWindow), new FrameworkPropertyMetadata(typeof(DialogViewModelWindow)));
        }


        public bool Close
        {
            get { return (bool)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.Register("Close", typeof(bool), typeof(DialogViewModelWindow),
            new UIPropertyMetadata(false, OnClosePropertyChanged));

        private static void OnClosePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DialogViewModelWindow)d).OnClosePropertyChanged();
        }

        private void OnClosePropertyChanged()
        {
            this.DialogResult = ViewModel.Result;
            Close();
        }

        void DialogViewModelWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }


        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);


        private void DialogViewModelWindow_SourceInitialized(object sender, EventArgs e)
        {
            SourceInitialized -= DialogViewModelWindow_SourceInitialized;

            var hWnd = new WindowInteropHelper(this);
            var sysMenu = GetSystemMenu(hWnd.Handle, false);
            EnableMenuItem(sysMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
        }

        private void DialogViewModelWindow_ContentRendered(object sender, EventArgs e)
        {
            ContentRendered -= DialogViewModelWindow_ContentRendered;

            if (Style == null || SizeToContent == System.Windows.SizeToContent.WidthAndHeight)
            {
                SizeToContent = System.Windows.SizeToContent.Manual;

                if (ResizeMode == System.Windows.ResizeMode.CanResize)
                {
                    if (VisualTreeHelper.GetChildrenCount(this) > 0)
                    {
                        FrameworkElement child = VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
                        if (child != null)
                        {
                            this.MaxHeight = child.MaxHeight;
                            this.MaxWidth = child.MaxWidth;
                            this.MinHeight = child.RenderSize.Height;
                            this.MinWidth = child.RenderSize.Width;
                        }
                    }
                }
            }
        }
    }
}
