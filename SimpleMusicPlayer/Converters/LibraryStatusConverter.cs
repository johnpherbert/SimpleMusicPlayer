using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using SimpleMusicPlayer.Models;


namespace SimpleMusicPlayer.Converters
{
    public class LibraryStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnStatus = string.Empty;

            if (value.GetType() == typeof(MusicLibraryStatus))
            {
                switch((MusicLibraryStatus)value)
                {
                    case MusicLibraryStatus.DONE:
                        returnStatus = "DONE";
                        break;
                    case MusicLibraryStatus.READING_DIRECTORY:
                        returnStatus = "Reading Directories";
                        break;
                    case MusicLibraryStatus.UPDATING_TAGS:
                        returnStatus = "Updating Tags";
                        break;
                    default:
                        break;
                }
            }

            return returnStatus;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
