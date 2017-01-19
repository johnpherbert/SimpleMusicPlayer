using GalaSoft.MvvmLight;
using SimpleMusicPlayer.Models.FileTree;
using System.Collections.ObjectModel;
using System;
using SimpleMusicPlayer.Services;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using SimpleMusicPlayer.Models;
using System.Collections.Generic;
using System.Collections;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using System.Windows.Forms;
using System.Windows;

namespace SimpleMusicPlayer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public MusicPlayerService MusicPlayer { get; set; } = new MusicPlayerService();

        public ObservableCollection<Item> Items { get; set; }

        private  Settings MusicPlayerSettings { get; set; }

        public ICommand OpenSettingsCommand { get; private set; }

        public ICommand ExitCommand { get; private set; }

        public ICommand PlaySongCommand { get; private set; }

        public ICommand PauseSongCommand { get; private set; }

        public ICommand StopSongCommand { get; private set; }

        public ICommand AddSongCommand { get; private set; }

        public ICommand RemoveSongCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            OpenSettingsCommand = new RelayCommand(() => OpenSettingsWindow());
            ExitCommand = new RelayCommand(() => Exit());

            PlaySongCommand = new RelayCommand<Song>((song) => MusicPlayer.PlaySong(song?.Path));
            PauseSongCommand = new RelayCommand(() => MusicPlayer.PauseSong());
            StopSongCommand = new RelayCommand(() => MusicPlayer.StopSong());

            AddSongCommand = new RelayCommand<object>((songs) => AddSong(songs));
            RemoveSongCommand = new RelayCommand<object>((playlist) => RemoveSong(playlist));

            Items = new ObservableCollection<Item>();

            // Load Music Player Settings
            MusicPlayerSettings = new Settings();            
            MusicPlayerSettings.Load();

            // Load the inital directories based on the settings
            LoadInitalDirectories(MusicPlayerSettings.MusicFolders);
        }

        public void AddSong(object parameter)
        {
            if (parameter?.GetType() == typeof(FileItem))
            {
                FileItem fi = parameter as FileItem;
                MusicPlayer.CurrentPlaylist.Add(new Song() { Name = fi.Name, Path = fi.Path });
            }
            else if (parameter?.GetType() == typeof(DirectoryItem))
            {
                DirectoryItem di = parameter as DirectoryItem;
                AddAllChildren(di);
            }
        }

        public void RemoveSong(object o)
        {

        }

        public void OpenSettingsWindow()
        {
            var settingsvm = SimpleIoc.Default.GetInstance<SettingsViewModel>();
            settingsvm.TempPlayerSettings = new Settings();
            settingsvm.TempPlayerSettings.Load();

            var settingview = new SettingsView()
            {
                DataContext = settingsvm
            };

            settingview.ShowDialog();
            if (settingsvm.Saved)
            {
                MusicPlayerSettings = new Settings();
                MusicPlayerSettings.Load();
                LoadInitalDirectories(MusicPlayerSettings.MusicFolders);
            }
        }

        public async void LoadInitalDirectories(IEnumerable paths)
        {
            Items = await DirectoryTreeService.ReadDirectoriesAsync(paths);            
            RaisePropertyChanged("Items");
       }

        /// Add all children from the folder.  Allows you to load a entire collection.
        /// </summary>
        /// <param name="di">The parent directory to load from.</param>
        private void AddAllChildren(DirectoryItem di)
        {
            foreach (object odi in di.Items)
            {
                if (odi.GetType() == typeof(DirectoryItem))
                {
                    DirectoryItem cdi = odi as DirectoryItem;
                    AddAllChildren(cdi);
                }
            }

            foreach (object ofi in di.Items)
            {
                if (ofi.GetType() == typeof(FileItem))
                {
                    FileItem cfi = ofi as FileItem;
                    MusicPlayer.CurrentPlaylist.Add(new Song() { Name = cfi.Name, Path = cfi.Path });
                }
            }
        }

        private void Exit()
        {
            MusicPlayer.Cleanup();
            MusicPlayer.StopSong();
            // window?.Close();
        }
    }
}