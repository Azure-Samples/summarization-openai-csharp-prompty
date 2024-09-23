using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SpeechRecognition
{
   public class ParametricBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                bool boolValue = (bool)value;
                bool invert = false;
                if (parameter != null)
                {
                    bool.TryParse(parameter.ToString(), out invert);
                }
                if (boolValue)
                {
                    return invert ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                }
                else
                {
                    return invert ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
                }
            }
            else
            {
                return System.Windows.Visibility.Collapsed;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
   
}
