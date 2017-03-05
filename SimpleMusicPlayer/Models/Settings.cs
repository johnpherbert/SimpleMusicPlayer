using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using SimpleMusicPlayer.Models.FileTree;

namespace SimpleMusicPlayer.Models
{
    public class Settings
    {
        private const string DEFAULT_FILENAME = "settings.json";

        public static Settings Default { get; } = new Settings();

        public ObservableCollection<DirectoryItem> MusicFolders { get; set; }

        public Settings()
        {            
            MusicFolders = new ObservableCollection<DirectoryItem>();
        }

        public void Save(string filename = DEFAULT_FILENAME)
        {
            File.WriteAllText(filename, (new JavaScriptSerializer()).Serialize(this));
        }

        public void Save(Settings settings, string filename = DEFAULT_FILENAME)
        {
            File.WriteAllText(filename, (new JavaScriptSerializer()).Serialize(settings));
        }

        public void Load(string filename = DEFAULT_FILENAME)
        {
            Settings loadedSettings = new Settings();
            if (File.Exists(filename))
                loadedSettings = (new JavaScriptSerializer()).Deserialize<Settings>(File.ReadAllText(filename));

            MusicFolders = loadedSettings?.MusicFolders;            
        }

        /* This is XML Serialization we are going with JSON
        public void LoadSettings(string filepath)
        {
            if(File.Exists(filepath))
            {
                Settings tempSettings;
                using (var s = new FileStream(filepath, FileMode.Open))
                {
                    var xml = new XmlSerializer(typeof(Settings));
                    tempSettings = (Settings)xml.Deserialize(s);
                }

                MusicFolders = new ObservableCollection<string>();
                MusicFolders = tempSettings?.MusicFolders;
            }
        }

        public void SaveSettings(string filepath)
        {
            using (var fs = new FileStream(filepath, FileMode.Create))
            {
                var xml = new XmlSerializer(typeof(Settings));
                xml.Serialize(fs, this);                    
            }
        }
        */
    }
}
