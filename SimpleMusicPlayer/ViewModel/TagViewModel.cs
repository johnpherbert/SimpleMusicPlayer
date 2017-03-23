﻿using System;
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
                Song id3song = TempID3Songs[0] as Song;
                
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