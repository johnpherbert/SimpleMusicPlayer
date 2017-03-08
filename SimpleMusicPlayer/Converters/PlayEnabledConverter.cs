using SimpleMusicPlayer.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimpleMusicPlayer.Converters
{
    public class PlayEnabledConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isButtonEnabled = true;

            if(value?.Length == 2 &&
               value[0].GetType() == typeof(int) &&
               value[1].GetType() == typeof(int) &&
               parameter.GetType() == typeof(string))
            {
                int playlistindex = (int)value[0];
                int playlistcount = (int)value[1];
                string func = (string)parameter;

                if(func == "Min")
                {
                    if (playlistindex == 0)
                        isButtonEnabled = false;
                }
                else if(func == "Max")
                {
                    if (playlistindex >= playlistcount - 1)
                        isButtonEnabled = false;
                }

                if (playlistcount == 0)
                    isButtonEnabled = false;
            }

            return isButtonEnabled;            
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
