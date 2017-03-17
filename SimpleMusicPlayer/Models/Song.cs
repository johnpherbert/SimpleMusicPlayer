using SimpleMusicPlayer.Models.FileTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SimpleMusicPlayer.Models
{

    public class Song : Item, INotifyPropertyChanged
    {
        private bool isbeingplayed;

        [ScriptIgnore]
        public bool IsBeingPlayed
        {
            get { return isbeingplayed; }
            set
            {
                isbeingplayed = value;
                NotifyProperyChanged("IsBeingPlayed");
            }
        }

        private SongInfo info;        
        public SongInfo Info
        {
            get { return info; }
            set
            {
                info = value;
                NotifyProperyChanged("Info");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyProperyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
