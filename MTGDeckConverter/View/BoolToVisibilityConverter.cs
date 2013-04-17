// -----------------------------------------------------------------------
// <copyright file="BoolToVisibilityConverter.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MTGDeckConverter.View
{
    /// <summary>
    /// Represents the converter that converts Boolean values to System.Windows.Visibility enumeration values, and can be set to return opposite values.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a Boolean value to a System.Windows.Visibility enumeration value.
        /// </summary>
        /// <param name="value">The Boolean value to convert. This value can be a standard Boolean value or a nullable Boolean value.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">If this is a string and equals 'opposite', then the opposite Visibility enumeration will be returned</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>System.Windows.Visibility.Visible if value is true; otherwise, System.Windows.Visibility.Collapsed.  Opposite if parameter is 'opposite'.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                bool b = (bool)value;
                string paramstring = parameter as string;

                if (paramstring != null && paramstring.Equals("opposite", StringComparison.InvariantCultureIgnoreCase))
                {
                    b = !b;
                }

                if (b)
                {
                    return Visibility.Visible;
                }
            }
            catch 
            { 
            }

            return Visibility.Collapsed;
        }

        /// <summary>
        /// This is not implemented.
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>Not Implemented Exception</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
