using System.Collections.ObjectModel;
using System.IO;
using System.Web.Script.Serialization;

namespace SimpleMusicPlayer.Models
{
    public enum MusicLibraryStatus
    {
        READING_DIRECTORY,
        UPDATING_TAGS,
        DONE
    }

    public class MusicLibraryManager
    {
        private const string DEFAULT_FILENAME = "library.json";

        public ObservableCollection<Song> Songs { get; set; } = new ObservableCollection<Song>();

        public void Save(string filename = DEFAULT_FILENAME)
        {
            File.WriteAllText(filename, (new JavaScriptSerializer()).Serialize(this));
        }

        public void Load(string filename = DEFAULT_FILENAME)
        {
            MusicLibraryManager loadedmusiclibrary = new MusicLibraryManager();
            if (File.Exists(filename))
                loadedmusiclibrary = (new JavaScriptSerializer()).Deserialize<MusicLibraryManager>(File.ReadAllText(filename));

            Songs = loadedmusiclibrary.Songs;
        }
    }
}
