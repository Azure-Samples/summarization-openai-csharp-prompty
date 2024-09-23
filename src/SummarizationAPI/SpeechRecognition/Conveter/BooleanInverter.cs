using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SpeechRecognition
{
    public class BooleanInverter : IValueConverter
    {

        public BooleanInverter()
        {
        }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                return !(bool)value;
            }
            else
            {
                return value;
            }
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Flip value again
            return this.Convert(value, targetType, parameter, culture);
        }
    }
}
