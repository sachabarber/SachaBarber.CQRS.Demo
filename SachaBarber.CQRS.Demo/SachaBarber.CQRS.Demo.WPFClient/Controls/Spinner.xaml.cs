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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SachaBarber.CQRS.Demo.WPFClient.Controls
{

    public partial class Spinner : UserControl
    {
        private readonly DoubleAnimation animationA = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(1.5)), FillBehavior.HoldEnd);
        private readonly DoubleAnimation animationB = new DoubleAnimation(0, -360, new Duration(TimeSpan.FromSeconds(2.0)), FillBehavior.HoldEnd);

        public Spinner()
        {
            InitializeComponent();
            animationA.RepeatBehavior = RepeatBehavior.Forever;
            animationB.RepeatBehavior = RepeatBehavior.Forever;

            IsVisibleChanged += HandleVisibleChanged;
        }

        private void HandleVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                spinnerArcA.BeginAnimation(RotateTransform.AngleProperty, animationA);
                spinnerArcB.BeginAnimation(RotateTransform.AngleProperty, animationB);
            }
            else
            {
                spinnerArcA.BeginAnimation(RotateTransform.AngleProperty, null);
                spinnerArcB.BeginAnimation(RotateTransform.AngleProperty, null);
            }
        }
    }
}
