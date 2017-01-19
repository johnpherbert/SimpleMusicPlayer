using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using SimpleMusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using SimpleMusicPlayer.Models.FileTree;
using System.Windows;

namespace SimpleMusicPlayer.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public bool Saved { get; set; } = false;

        public Settings TempPlayerSettings { get; set; }

        public ICommand SaveSettingsCommand { get; private set; }

        public ICommand CancelSettingsCommand { get; private set; }

        public ICommand AddMusicPathCommand { get; private set; }

        public ICommand SearchPathCommand { get; private set; }

        public ICommand RemoveMusicPathCommand { get; private set; }

        public SettingsViewModel()
        {
            SaveSettingsCommand = new RelayCommand<Window>((window) => SaveSettings(window));
            CancelSettingsCommand = new RelayCommand<Window>((window) => CancelSettings(window));
            AddMusicPathCommand = new RelayCommand(() => AddMusicPath());
            SearchPathCommand = new RelayCommand<DirectoryItem>((path) => SearchPath(path));
            RemoveMusicPathCommand = new RelayCommand<DirectoryItem>((path) => RemoveMusicPath(path));
        }

        public void SaveSettings(Window settingswindow)
        {
            Saved = true;
            TempPlayerSettings.Save();
            settingswindow?.Close();
        }

        public void CancelSettings(Window settingswindow)
        {
            Saved = false;
            settingswindow?.Close();
        }

        public void AddMusicPath()
        {
            DirectoryItem newpath = OpenFileDialog(new DirectoryItem());
            if(!TempPlayerSettings.MusicFolders.Contains(newpath))
                TempPlayerSettings.MusicFolders.Add(newpath);
        }

        public void RemoveMusicPath(DirectoryItem musicpath)
        {
            TempPlayerSettings.MusicFolders.Remove(musicpath);
        }

        public void SearchPath(DirectoryItem musicpath)
        {
            DirectoryItem newpath = OpenFileDialog(musicpath);
            int index = TempPlayerSettings.MusicFolders.IndexOf(musicpath);
            TempPlayerSettings.MusicFolders[index] = newpath;
        }

        private DirectoryItem OpenFileDialog(DirectoryItem basepath)
        {
            DirectoryItem returnpath = basepath;
                        
            FolderBrowserDialog fbd = new FolderBrowserDialog() { SelectedPath = basepath.Path };
            if (fbd.ShowDialog() == DialogResult.OK)
                returnpath.Path = fbd.SelectedPath;

            return returnpath;             
        }
    }
}
