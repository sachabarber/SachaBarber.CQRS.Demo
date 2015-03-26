using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SachaBarber.CQRS.Demo.WPFClient.Controls
{
    public enum AsyncType
    {
        Content,
        Busy,
        Error
    }


    public class AsyncHostControl : ContentControl
    {
        static AsyncHostControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AsyncHostControl), new FrameworkPropertyMetadata(typeof(AsyncHostControl)));
        }

        public AsyncType AsyncState
        {
            get { return (AsyncType)GetValue(AsyncStateProperty); }
            set { SetValue(AsyncStateProperty, value); }
        }

        public static readonly DependencyProperty AsyncStateProperty =
            DependencyProperty.Register("AsyncState", typeof(AsyncType), typeof(AsyncHostControl), new UIPropertyMetadata(AsyncType.Busy));

        public bool ShouldCollapse
        {
            get { return (bool)GetValue(ShouldCollapseProperty); }
            set { SetValue(ShouldCollapseProperty, value); }
        }

        public static readonly DependencyProperty ShouldCollapseProperty =
            DependencyProperty.Register("ShouldCollapse", typeof(bool), typeof(AsyncHostControl), new UIPropertyMetadata(false));

        public object BusyContent
        {
            get { return (object)GetValue(BusyContentProperty); }
            set { SetValue(BusyContentProperty, value); }
        }

        public static readonly DependencyProperty BusyContentProperty =
            DependencyProperty.Register("BusyContent", typeof(object), typeof(AsyncHostControl));

        public object ErrorContent
        {
            get { return (object)GetValue(ErrorContentProperty); }
            set { SetValue(ErrorContentProperty, value); }
        }

        public static readonly DependencyProperty ErrorContentProperty =
            DependencyProperty.Register("ErrorContent", typeof(object), typeof(AsyncHostControl));

        public DataTemplate BusyTemplate
        {
            get { return (DataTemplate)GetValue(BusyTemplateProperty); }
            set { SetValue(BusyTemplateProperty, value); }
        }

        public static readonly DependencyProperty BusyTemplateProperty =
            DependencyProperty.Register("BusyTemplate", typeof(DataTemplate), typeof(AsyncHostControl));

        public DataTemplate ErrorTemplate
        {
            get { return (DataTemplate)GetValue(ErrorTemplateProperty); }
            set { SetValue(ErrorTemplateProperty, value); }
        }

        public static readonly DependencyProperty ErrorTemplateProperty =
            DependencyProperty.Register("ErrorTemplate", typeof(DataTemplate), typeof(AsyncHostControl));
    }  
}
