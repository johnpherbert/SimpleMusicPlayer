using SimpleMusicPlayer.Models;
using SimpleMusicPlayer.Models.FileTree;
using SimpleMusicPlayer.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleMusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Currentplaylistview_Drop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(typeof(Song)))
            {
                ((MainViewModel)DataContext).AddSong(e.Data.GetData(typeof(Song)));
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
        }

        // private void Filetree_PreviewMouseMove(object sender, MouseEventArgs e)
        // {
        //     TreeView tv = (TreeView)sender;
        //     if (tv?.SelectedItem != null && e.LeftButton == MouseButtonState.Pressed)
        //         DragDrop.DoDragDrop(filetree, tv.SelectedItem, DragDropEffects.Copy);
        // }

        private void Playlistview_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // TODO allow multiple playlists to be added at once

            ListView tv = (ListView)sender;
            if (tv?.SelectedItem != null && e.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop(playlistview, tv.SelectedItem, DragDropEffects.Copy);
        }

        private void Currentplaylistview_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && e.IsDown == true)
            {
                ((MainViewModel)DataContext).RemoveSong(currentplaylistview.SelectedItems);
            }
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
            if (tv?.SelectedItem != null && e.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop(filesongslistview, tv.SelectedItem, DragDropEffects.Copy);
        }

        private void Filesongslistview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView tv = (ListView)sender;
            Song song = tv.SelectedItem as Song;
            ((MainViewModel)DataContext).AddSong(tv.SelectedItem);
            ((MainViewModel)DataContext).MusicPlayer.PlayNewSong(song.Path);
        }
        
    }
}
