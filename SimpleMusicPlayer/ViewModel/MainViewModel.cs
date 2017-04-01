using GalaSoft.MvvmLight;
using SimpleMusicPlayer.Models.FileTree;
using System.Collections.ObjectModel;
using System;
using System.Linq;
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
using System.Windows.Controls;
using System.Windows.Data;

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
        public MusicLibraryManager MusicLibraryManager { get; set; } = new MusicLibraryManager();

        public PlaylistManager PlaylistManager { get; set; } = new PlaylistManager();

        public MusicPlayerService MusicPlayer { get; set; } = new MusicPlayerService();

        public CollectionView SongView { get; set; }

        private Settings MusicPlayerSettings { get; set; }

        public string PlaylistName { get; set; }

        private MusicLibraryStatus librarystatus;
        public MusicLibraryStatus LibraryStatus
        {
            get
            {
                return librarystatus;
            }

            set
            {
                librarystatus = value;
                RaisePropertyChanged("LibraryStatus");
            }
        }

        private string filterstring;
        public string FilterString
        {
            get
            {
                return filterstring;
            }
            set
            {
                filterstring = value;
                RaisePropertyChanged("FilterString");
                SongView.Refresh();
            }
        }

        public ICommand OpenSettingsCommand { get; private set; }

        public ICommand ExitCommand { get; private set; }

        public ICommand PlaySongCommand { get; private set; }

        public ICommand PlayPreviousSongCommand { get; private set; }

        public ICommand PlayNextSongCommand { get; private set; }

        public ICommand PauseSongCommand { get; private set; }

        public ICommand StopSongCommand { get; private set; }

        public ICommand AddSongCommand { get; private set; }

        public ICommand RemoveSongCommand { get; private set; }

        public ICommand CreatePlayListCommand { get; private set; }

        public ICommand DeletePlaylistCommand { get; private set; }

        public ICommand EditTagCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {

            // string path = @"E:\Music\Ace of Base\Flowers\17 - Cruel Summer.mp3";
            // MusicTagReaderService.ReadSong(path);

            OpenSettingsCommand = new RelayCommand(() => OpenSettingsWindow());
            ExitCommand = new RelayCommand(() => Exit());

            PlaySongCommand = new RelayCommand<Song>((song) => MusicPlayer.PlaySong(song?.Path));
            PlayNextSongCommand = new RelayCommand(() => MusicPlayer.PlayNextSong());
            PlayPreviousSongCommand = new RelayCommand(() => MusicPlayer.PlayPreviousSong());

            PauseSongCommand = new RelayCommand(() => MusicPlayer.PauseSong());
            StopSongCommand = new RelayCommand(() => MusicPlayer.StopSong());

            CreatePlayListCommand = new RelayCommand<System.Windows.Controls.ListView>((songs) => CreatePlaylist(songs));
            DeletePlaylistCommand = new RelayCommand<Playlist>((songs) => DeletePlaylist(songs));

            AddSongCommand = new RelayCommand<object>((songs) => AddSong(songs));
            RemoveSongCommand = new RelayCommand<IList>((playlist) => RemoveSong(playlist));

            EditTagCommand = new RelayCommand<IList>((songs) => OpenTagWindow(songs));

            // Load Music Player Settings
            MusicPlayerSettings = new Settings();
            MusicPlayerSettings.Load();

            // Load the MusicLibrary
            LoadMusicLibrary();            

            // Load the inital directories based on the settings
            LoadInitalDirectories(MusicPlayerSettings.MusicFolders);

            PlaylistManager.Load();
        }

        public void CreatePlaylist(System.Windows.Controls.ListView playlistview)
        {
            Playlist createpl = new Playlist() { Name = "New Playlist" };

            foreach (Song s in playlistview.Items)
                createpl.Songs.Add(s);

            PlaylistManager.Playlists.Add(createpl);
            PlaylistManager.Save();
        }

        public void DeletePlaylist(Playlist playlist)
        {
            PlaylistManager.Remove(playlist);
        }

        public void AddSong(object parameter)
        {
            if(parameter?.GetType() == typeof(List<Song>))
            {
                List<Song> songlist = parameter as List<Song>;
                foreach (Song s in songlist)
                    MusicPlayer.CurrentPlaylist.Add(s);
            }
            else if (parameter?.GetType() == typeof(List<Playlist>))
            {
                List<Playlist> playlists = parameter as List<Playlist>;
                foreach (Playlist pl in playlists)
                {
                    foreach(Song s in pl.Songs)
                        MusicPlayer.CurrentPlaylist.Add(s);
                }                    
            }
            else if (parameter?.GetType() == typeof(Song))
            {
                Song fi = parameter as Song;
                MusicPlayer.CurrentPlaylist.Add(fi);
            }
            else if (parameter?.GetType() == typeof(FileItem))
            {
                FileItem fi = parameter as FileItem;
                MusicPlayer.CurrentPlaylist.Add(new Song() { Name = fi.Name, Path = fi.Path });
            }
            else if (parameter?.GetType() == typeof(DirectoryItem))
            {
                DirectoryItem di = parameter as DirectoryItem;
                AddAllChildren(di);
            }
            else if (parameter?.GetType() == typeof(Playlist))
            {
                Playlist pl = parameter as Playlist;
                foreach (Song s in pl.Songs)
                    MusicPlayer.CurrentPlaylist.Add(s);
            }
        }

        public void RemoveSong(IList filestoremove)
        {
            for (int x = filestoremove.Count - 1; x >= 0; x--)
            {
                Song songtoremove = filestoremove[x] as Song;
                if (MusicPlayer.CurrentPlaylist.Contains(songtoremove))
                    MusicPlayer.CurrentPlaylist.Remove(songtoremove);
            }
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

        public void OpenTagWindow(IList songs)
        {
            var tagvm = SimpleIoc.Default.GetInstance<TagViewModel>();
            tagvm.InitalizeTagView(songs);

            var tagv = new TagView()
            {
                DataContext = tagvm
            };

            tagv.ShowDialog();

            if (tagvm.Saved)
            {
                foreach (Song s in tagvm.TempID3Songs)
                {
                    if (MusicLibraryManager.Songs.Contains(s))
                        MusicLibraryManager.Songs[MusicLibraryManager.Songs.IndexOf(s)].Liked = s.Liked;
                }
            }
        }

        public void LoadMusicLibrary()
        {
            // Load the songs we already know about
            MusicLibraryManager.Load();
            SongView = (CollectionView)CollectionViewSource.GetDefaultView(MusicLibraryManager.Songs);
            SongView.Filter = CustomerFilter;
            RaisePropertyChanged("SongView");
            LibraryStatus = MusicLibraryStatus.DONE;
        }

        public async void LoadInitalDirectories(IEnumerable paths)
        {
            // We now check for real if those songs exist.
            LibraryStatus = MusicLibraryStatus.READING_DIRECTORY;
            // First pass to just get the songs in the player
            MusicLibraryManager.Songs = await DirectoryTreeService.ReadSongsAsync(paths, MusicLibraryManager.Songs);
            SongView = (CollectionView)CollectionViewSource.GetDefaultView(MusicLibraryManager.Songs);
            SongView.Filter = CustomerFilter;
            SongView.Refresh();
            RaisePropertyChanged("SongView");        

            LibraryStatus = MusicLibraryStatus.UPDATING_TAGS;
            // Second pass to update it with the correct ID3 tags
            MusicLibraryManager.Songs = await MusicTagReaderService.UpdateSongInfoAsync(MusicLibraryManager.Songs);
            SongView.Refresh();
            RaisePropertyChanged("SongView");

            MusicLibraryManager.Save();
            LibraryStatus = MusicLibraryStatus.DONE;
        }

        public bool CustomerFilter(object songitem)
        {
            bool returnval = false;
            Song song = songitem as Song;

            if (!string.IsNullOrEmpty(FilterString))
            {
                if(FilterString == "***")
                {
                    if (song.Liked)
                        return true;
                }
                else if (song?.Info?.SongTitle?.Replace("-","").IndexOf(FilterString.Trim(), 0, StringComparison.OrdinalIgnoreCase) != -1 ||
                   song?.Info?.Album?.Replace("-", "").IndexOf(FilterString.Trim(), 0, StringComparison.OrdinalIgnoreCase) != -1 ||
                   song?.Info?.Artist?.Replace("-", "").IndexOf(FilterString.Trim(), 0, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    returnval = true;
                }
                else
                {
                    returnval = false;
                }
            }
            else
            {
                returnval = true;
            }

            //Return members whose Orders have not been filled
            return returnval;
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
            MusicLibraryManager.Save();

            MusicPlayer.Cleanup();
            MusicPlayer.StopSong();
            // window?.Close();
        }
    }
}