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
using System.Windows.Media;
using static SimpleMusicPlayer.CustomControls.ListViewMultiSelect;

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

        private void Playlistview_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ListView tv = playlistview;                        
            var srcType = e.Source.GetType();

            if (tv?.SelectedItems != null && e.LeftButton == MouseButtonState.Pressed && srcType != typeof(TextBox))
            {
                List<Playlist> selectedplaylists = tv.SelectedItems.Cast<Playlist>().ToList();
                DragDrop.DoDragDrop(playlistview, selectedplaylists, DragDropEffects.Copy);
            }
        }

        private void Currentplaylistview_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && e.IsDown == true)            
                ((MainViewModel)DataContext).RemoveSong(currentplaylistview.SelectedItems);            
        }

        private void Currentplaylistview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Make sure we are not double clicking the scrollviewer
            var src = VisualTreeHelper.GetParent((DependencyObject)e.OriginalSource);
            var srcType = src.GetType();

            if (srcType != typeof(System.Windows.Controls.Primitives.RepeatButton) && srcType != typeof(System.Windows.Controls.Primitives.Thumb))
            {
                Song s = currentplaylistview.SelectedItem as Song;
                ((MainViewModel)DataContext).MusicPlayer.PlayNewSong(s.Path, false);
            }
        }

        private void Playlisttextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).PlaylistManager.Save();
        }

        private void Filesongslistview_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // If its a checkbox just ignore
            var srcType = e.Source.GetType();            

            ListView tv = filesongslistview;

            if (tv?.SelectedItems != null && e.LeftButton == MouseButtonState.Pressed && srcType != typeof(CheckBox))
            {
                List<Song> selectedsongs = tv.SelectedItems.Cast<Song>().ToList();
                DragDrop.DoDragDrop(filesongslistview, selectedsongs, DragDropEffects.Copy);
            }
        }

        private void Filesongslistview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Make sure we are not double clicking the scrollviewer
            var src = VisualTreeHelper.GetParent((DependencyObject)e.OriginalSource);
            var srcType = src.GetType();

            if (srcType != typeof(System.Windows.Controls.Primitives.RepeatButton) && srcType != typeof(System.Windows.Controls.Primitives.Thumb))
            {
                ListView tv = (ListView)sender;
                Song song = tv.SelectedItem as Song;
                ((MainViewModel)DataContext).AddSong(tv.SelectedItem);
                ((MainViewModel)DataContext).MusicPlayer.PlayNewSong(song?.Path, false);
            }           
        }
    }
}
