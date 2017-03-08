using SimpleMusicPlayer.Models;
using SimpleMusicPlayer.Models.FileTree;
using SimpleMusicPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void currentplaylistview_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DirectoryItem)))
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

        private void currentplaylistview_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void filetree_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            TreeView tv = (TreeView)sender;
            if (tv?.SelectedItem != null && e.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop(filetree, tv.SelectedItem, DragDropEffects.Copy);
        }

        private void playlistview_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // TODO allow multiple playlists to be added at once

            ListView tv = (ListView)sender;
            if (tv?.SelectedItem != null && e.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop(playlistview, tv.SelectedItem, DragDropEffects.Copy);
        }

        private void currentplaylistview_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && e.IsDown == true)
            {
                ((MainViewModel)DataContext).RemoveSong(currentplaylistview.SelectedItems);
            }
        }

        private void currentplaylistview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Song s = currentplaylistview.SelectedItem as Song;
            ((MainViewModel)DataContext).MusicPlayer.PlayNewSong(s.Path);
        }

        private void playlisttextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).PlaylistManager.Save();
        }
    }
}
