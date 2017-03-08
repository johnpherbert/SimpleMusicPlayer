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
    public class PlayPauseVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility returnVisibility = Visibility.Visible;

            if (value.GetType() == typeof(bool) && parameter.GetType() == typeof(string))
            {
                bool isplaying = (bool)value;
                string playpause = (string)parameter;

                if (isplaying && playpause == "play")
                {
                    returnVisibility = Visibility.Hidden;
                }
                else if (!isplaying && playpause == "play")
                {
                    returnVisibility = Visibility.Visible;
                }
                else if (isplaying && playpause == "pause")
                {
                    returnVisibility = Visibility.Visible;
                }
                else if (!isplaying && playpause == "pause")
                {
                    returnVisibility = Visibility.Hidden;
                }
            }

            return returnVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
