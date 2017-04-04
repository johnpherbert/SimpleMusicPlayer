using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SimpleMusicPlayer.Models;
using System.Windows.Input;
using System.Windows.Forms;
using SimpleMusicPlayer.Models.FileTree;
using System.Windows;
using System.Collections;
using System.Collections.ObjectModel;

using CSCore.Tags.ID3;
using TagLib;

namespace SimpleMusicPlayer.ViewModel
{
    public class TagViewModel : ViewModelBase
    {
        public bool Saved { get; set; } = false;

        public IList TempID3Songs { get; set; }

        public string Artist { get; set; }

        public bool CanEditArtist { get; set; } = false;

        public string Album { get; set; }

        public bool CanEditAlbum { get; set; } = false;

        public string SongTitle { get; set; }

        public bool IsLiked { get; set; }

        public bool CanEditSongTitle { get; set; } = false;

        public ICommand SaveSettingsCommand { get; private set; }

        public ICommand CancelSettingsCommand { get; private set; }

        public TagViewModel()
        {
            SaveSettingsCommand = new RelayCommand<Window>((window) => SaveSettings(window));
            CancelSettingsCommand = new RelayCommand<Window>((window) => CancelSettings(window));
        }

        public void InitalizeTagView(IList songs)
        {
            TempID3Songs = songs;            
            
            if (songs.Count > 0)
            {
                CanEditSongTitle = false;

                Song id3song = TempID3Songs[0] as Song;

                IsLiked = id3song.Liked;

                if (songs.Count == 1)
                {
                    SongTitle = id3song.Info.SongTitle;
                    CanEditSongTitle = true;                    
                }
                

                Artist = id3song.Info.Artist;
                CanEditArtist = true;

                Album = id3song.Info.Album;
                CanEditAlbum = true;
            }
        }

        public void SaveSettings(Window tagwindow)
        {
            foreach (Song song in TempID3Songs)
            {
                var songfile = File.Create(song.Path);

                if (CanEditSongTitle)
                {
                    songfile.Tag.Title = SongTitle;
                    song.Info.SongTitle = SongTitle;
                }

                if (CanEditArtist)
                {
                    string[] artists = songfile.Tag.Performers;
                    artists[0] = Artist;
                    songfile.Tag.Performers = artists;
                    song.Info.Artist = Artist;
                }

                if (CanEditAlbum)
                {
                    songfile.Tag.Album = Album;
                    song.Info.Album = Album;
                }

                song.Liked = IsLiked;

                songfile.Save();
            }

            Saved = true;            
            tagwindow?.Close();                       
        }

        public void CancelSettings(Window tagwindow)
        {
            Saved = false;
            tagwindow?.Close();
        }
    }
}
