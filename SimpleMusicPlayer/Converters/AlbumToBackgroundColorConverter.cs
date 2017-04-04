using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace SimpleMusicPlayer.Converters
{
    public class AlbumToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush returnBrush = Brushes.Transparent;

            if (value?.GetType() == typeof(string))
            {
                string albumname = value as string;
                int seed = 0;
                foreach (char c in albumname)
                    seed += (int)c;

                Random rnd = new Random(seed);
                
                int red =  rnd.Next(0, 255);                
                int green = rnd.Next(0, 255);
                int blue = rnd.Next(0, 255);

                returnBrush = new SolidColorBrush(Color.FromArgb(255, System.Convert.ToByte(red), System.Convert.ToByte(green), System.Convert.ToByte(blue)));
            }

            return returnBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
