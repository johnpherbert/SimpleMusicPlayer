using SimpleMusicPlayer.Models;
using SimpleMusicPlayer.Models.FileTree;
using SimpleMusicPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SimpleMusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<object> PreviouslySelectedSongs = new List<object>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Currentplaylistview_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(List<Song>)))
            {
                ((MainViewModel)DataContext).AddSong(e.Data.GetData(typeof(List<Song>)));
            }
            if (e.Data.GetDataPresent(typeof(List<Playlist>)))
            {
                ((MainViewModel)DataContext).AddSong(e.Data.GetData(typeof(List<Playlist>)));
            }
            else if (e.Data.GetDataPresent(typeof(DirectoryItem)))
            {
                ((MainViewModel)DataContext).AddSong(e.Data.GetData(typeof(DirectoryItem)));
            }
            else if (e.Data.GetDataPresent(typeof(FileItem)))
            {
                ((MainViewModel)DataContext).AddSong(e.Data.GetData(typeof(FileItem)));
            }
            else if (e.Data.GetDataPresent(typeof(Playlist)))
            {
                ((MainViewModel)DataContext).AddSong(e.Data.GetData(typeof(Playlist)));
            }
            else if (e.Data.GetDataPresent(typeof(Song)))
            {
                ((MainViewModel)DataContext).AddSong(e.Data.GetData(typeof(Song)));
            }
        }

        // private void Filetree_PreviewMouseMove(object sender, MouseEventArgs e)
        // {
        //     TreeView tv = (TreeView)sender;
        //     if (tv?.SelectedItem != null && e.LeftButton == MouseButtonState.Pressed)
        //         DragDrop.DoDragDrop(filetree, tv.SelectedItem, DragDropEffects.Copy);
        // }

        private void Playlistview_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ListView tv = (ListView)sender;
            if (tv?.SelectedItem != null && e.LeftButton == MouseButtonState.Pressed)
            {
                List<Playlist> selectedplaylist = tv.SelectedItems.Cast<Playlist>().ToList();
                DragDrop.DoDragDrop(playlistview, selectedplaylist, DragDropEffects.Copy);
            }
        }

        private void Currentplaylistview_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && e.IsDown == true)            
                ((MainViewModel)DataContext).RemoveSong(currentplaylistview.SelectedItems);            
        }

        private void Currentplaylistview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Song s = currentplaylistview.SelectedItem as Song;
            ((MainViewModel)DataContext).MusicPlayer.PlayNewSong(s.Path);
        }

        private void Playlisttextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).PlaylistManager.Save();
        }

        private void Filesongslistview_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ListView tv = (ListView)sender;
            if (tv?.SelectedItems != null && e.LeftButton == MouseButtonState.Pressed)
            {
                List<Song> selectedsongs = tv.SelectedItems.Cast<Song>().ToList();
                DragDrop.DoDragDrop(filesongslistview, selectedsongs, DragDropEffects.Copy);
            }
        }        

        private void Filesongslistview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView tv = (ListView)sender;
            Song song = tv.SelectedItem as Song;
            ((MainViewModel)DataContext).AddSong(tv.SelectedItem);
            ((MainViewModel)DataContext).MusicPlayer.PlayNewSong(song?.Path);            
        }

        private bool UserFilter(object item)
        {
            bool returnval = false;

            if(!string.IsNullOrEmpty(filterbox.Text))
            {
                string filter = filterbox.Text;
                returnval = true;

                Song song = item as Song;

                returnval = song.Info.Artist.Contains(filter) || song.Info.SongTitle.Contains(filter) || song.Info.Album.Contains(filter);
            }

            return returnval;
        }

        private void Filterbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ((MainViewModel)DataContext).AddSong(tv.SelectedItem);
            // CollectionViewSource.GetDefaultView(filesongslistview.ItemsSource).Refresh();
        }
    }
}
