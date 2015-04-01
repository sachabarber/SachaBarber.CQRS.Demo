using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SachaBarber.CQRS.Demo.WPFClient.Converters
{
    [ValueConversion(typeof(Boolean), typeof(Visibility))]
    public class BoolToVisibilityCollapsedConverter : IValueConverter
    {

        private BoolToVisibilityCollapsedConverter()
        {

        }

        static BoolToVisibilityCollapsedConverter()
        {
            Instance = new BoolToVisibilityCollapsedConverter();
        }

        public static BoolToVisibilityCollapsedConverter Instance { get; private set; }


        #region IValueConverter implementation
        /// <summary>
        /// Converts Boolean to Visibility
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            if (parameter == null)
                return Binding.DoNothing;

            bool input = false;
            bool.TryParse(value.ToString(), out input);

            bool invertActive = false;
            bool.TryParse(parameter.ToString(), out invertActive);

            if (input)
            {
                return invertActive ? Visibility.Visible : Visibility.Collapsed;
            }

            return invertActive ? Visibility.Collapsed : Visibility.Visible;

        }

        /// <summary>
        /// Convert back, but its not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Not implemented");
        }
        #endregion
    }
}
