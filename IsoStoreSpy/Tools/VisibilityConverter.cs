using System;
using System.Windows;
using System.Windows.Data;

namespace IsoStoreSpy.Tools
{
    public class VisibilityConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Convertion de boolean vers une visibility -> True = Visible, False = Collapsed, 
        /// Un parametre = à true permet d'inverser le resultat
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool mustInvert = false;

            if (parameter != null)
            {
                bool.TryParse(parameter as string, out mustInvert);
            }

            if ((value is int) == true)
            {
                value = !((int)value == 0);
            }

            if (value == null)
            {
                value = false;
            }
            
            if (value is bool == false)
            {
                value = true;
            }

            if ((bool)value == true)
            {
                if (mustInvert == false)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }

            if (mustInvert == false)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        /// <summary>
        /// Convertion retour
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
