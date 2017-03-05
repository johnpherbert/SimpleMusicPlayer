using SimpleMusicPlayer.Models.FileTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMusicPlayer.Models
{
    public class Song : Item, INotifyPropertyChanged
    {
        private bool isbeingplayed;
        public bool IsBeingPlayed
        {
            get { return isbeingplayed; }
            set
            {
                isbeingplayed = value;
                NotifyProperyChanged("IsBeingPlayed");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyProperyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
