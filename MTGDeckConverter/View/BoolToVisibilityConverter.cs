using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace MTGDeckConverter.View
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        //If parameter equals 'opposite', then the opposite will be returned
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Diagnostics.Debug.Assert(targetType == typeof(Visibility));
            try
            {
                bool b = (bool)value;
                string paramstring = parameter as string;
                if (paramstring != null && paramstring.Equals("opposite", StringComparison.InvariantCultureIgnoreCase))
                {
                    b = !b;
                }
                if (b) return Visibility.Visible;
            }
            catch { }


            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
