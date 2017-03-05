using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMusicPlayer.Models
{
    public class Playlist
    {
        public string Name { get; set; }

        public ObservableCollection<Song> Songs { get; set; } = new ObservableCollection<Song>();

        public Playlist()
        {

        }
    }
}
