using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SimpleMusicPlayer.Models
{
    public class PlaylistManager
    {
        private const string DEFAULT_FILENAME = "playlist.json";

        public ObservableCollection<Playlist> Playlists { get; set; } = new ObservableCollection<Playlist>();        

        public void Remove(Playlist playlist)
        {
            foreach(Playlist pl in Playlists)
            {
                if(pl == playlist)
                {
                    Playlists.Remove(pl);
                    break;
                }
            }

            Save();
        }

        public void Save(string filename = DEFAULT_FILENAME)
        {
            File.WriteAllText(filename, (new JavaScriptSerializer()).Serialize(this));
        }

        public void Load(string filename = DEFAULT_FILENAME)
        {
            PlaylistManager loadedplaylistman = new PlaylistManager();
            if (File.Exists(filename))
                loadedplaylistman = (new JavaScriptSerializer()).Deserialize<PlaylistManager>(File.ReadAllText(filename));

            Playlists = loadedplaylistman.Playlists;
        }
    }
}
