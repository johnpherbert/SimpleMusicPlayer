using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace SimpleMusicPlayer.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility reutrnVisiblity = Visibility.Hidden;

            if (value.GetType() == typeof(bool))
            {
                bool visiblity = (bool)value;
                if (visiblity)
                    reutrnVisiblity =  Visibility.Visible;
            }

            return reutrnVisiblity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
