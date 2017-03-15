using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore;
using CSCore.SoundOut;
using System.Collections.ObjectModel;
using SimpleMusicPlayer.Models.FileTree;
using System.IO;
using SimpleMusicPlayer.Models;
using System.ComponentModel;
using System.Windows.Data;

namespace SimpleMusicPlayer.Services
{
    public enum PlayerStopped
    {
        /// <summary>
        /// The song was over.
        /// </summary>
        SongOver,

        /// <summary>
        /// The user specifically stopped the song.
        /// </summary>
        UserStopped,

        /// <summary>
        /// The user paused the song
        /// </summary>
        UserPaused
    };

    public enum PlayerStatus
    {
        /// <summary>
        /// The player is stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// The player is playing a song
        /// </summary>
        Playing,

        /// <summary>
        /// The player is paused
        /// </summary>
        Paused
    };

    public class MusicPlayerService : INotifyPropertyChanged
    {
        public PlayerStopped HowPlayerStopped = PlayerStopped.SongOver;

        public PlayerStatus MusicPlayerStatus = PlayerStatus.Stopped;

        public ObservableCollection<Song> CurrentPlaylist { get; set; } = new ObservableCollection<Song>();

        private bool isplayingsong;
        public bool IsPlayingSong
        {
            get { return isplayingsong; }

            set
            {
                isplayingsong = value;
                NotifyProperyChanged("IsPlayingSong");
            }
        }

        private TimeSpan currentsonglength;
        public TimeSpan CurrentSongLength
        {
            get { return currentsonglength; }

            set
            {
                currentsonglength = value;
                NotifyProperyChanged("CurrentSongLength");
            }
        }

        public TimeSpan CurrentTime
        {
            get
            {
                if (SoundOut != null && SoundOut.WaveSource != null)
                    return SoundOut.WaveSource.GetTime(SoundOut.WaveSource.Position);
                else
                    return new TimeSpan(0);
            }
        }

        private string currentsong;
        public string CurrentSong
        {
            get { return currentsong; }

            set
            {
                currentsong = value;
                NotifyProperyChanged("CurrentSong");
            }
        }

        private int playlistindex;
        public int PlayListIndex
        {
            get
            {
                return playlistindex;
            }

            set
            {
                playlistindex = value;
                NotifyProperyChanged("PlayListIndex");
            }
        }

        private float volume;
        public float Volume
        {
            get { return volume; }

            set
            {
                volume = value;

                // The slider scale is 0-100 but the library scale is 0-1 so we need to * .01 to get the correct value.
                if (SoundOut?.WaveSource != null)
                    SoundOut.Volume = value * 0.01f;
            }
        }

        public ISoundOut SoundOut;
        public IWaveSource WaveSource;

        public event PropertyChangedEventHandler PropertyChanged;

        public MusicPlayerService()
        {
            Volume = 20f;

            PlayListIndex = 0;
            IsPlayingSong = false;        

            if (WasapiOut.IsSupportedOnCurrentPlatform)
                SoundOut = new WasapiOut();
            else
                SoundOut = new DirectSoundOut();

            SoundOut.Stopped += SoundOut_Stopped;
        }

        private void SoundOut_Stopped(object sender, PlaybackStoppedEventArgs e)
        {
            WasapiOut stop = sender as WasapiOut;

            // If the song stopped because the song was over and not because the user stopped it.
            if (HowPlayerStopped != PlayerStopped.UserStopped)
            {
                // Play the next song.
                PlayListIndex++;

                // If the playlist is at the end loop to the beginning.
                if (CurrentPlaylist.Count <= PlayListIndex)
                    PlayListIndex = 0;

                if (PlayListIndex < CurrentPlaylist.Count)
                    PlayNewSong(CurrentPlaylist[PlayListIndex].Path);
            }

            // User may have stopped it so we reset to song over.
            HowPlayerStopped = PlayerStopped.SongOver;
        }

        public void PlayNewSong(string path = "")
        {
            // If we have selected a diffrent song we should play it.                
            if (CurrentPlaylist.Count > 0)
            {
                string songtoplay = path;

                if (string.IsNullOrEmpty(path))
                    songtoplay = CurrentPlaylist[0].Path;

                PlayListIndex = GetPlaylistIndexFromPath(songtoplay);


                // If the song was playing stop it before we start a new song.
                if (SoundOut.PlaybackState == PlaybackState.Playing || SoundOut.PlaybackState == PlaybackState.Paused)
                {
                    HowPlayerStopped = PlayerStopped.UserStopped;
                    SoundOut.Stop();
                }

                WaveSource = CSCore.Codecs.CodecFactory.Instance.GetCodec(songtoplay);
                SoundOut.Initialize(WaveSource);

                // Volume resets on each play so we need to set it to the proper level again.
                SoundOut.Volume = volume * 0.01f;
                CurrentSong = new FileInfo(songtoplay).Name;

                SetSongAsPlayed(PlayListIndex);
                CurrentSongLength = SoundOut.WaveSource.GetTime(SoundOut.WaveSource.Length);
                IsPlayingSong = true;
                MusicPlayerStatus = PlayerStatus.Playing;
                SoundOut.Play();
            }
        }

        public void PlaySong(string path = "")
        {
            // If the song is playing and we are on the same song we need to pause
            if (MusicPlayerStatus == PlayerStatus.Playing)
            {
                PauseSong();
            }
            // If we are paused and we are on the same song Resume the song.
            else if (MusicPlayerStatus == PlayerStatus.Paused)
            {
                ResumeSong();
            }
            else
            {
                PlayNewSong(path);
            }
        }

        public void PlayNextSong()
        {
            if (PlayListIndex < CurrentPlaylist.Count - 1)
            {
                // We dont want to adjust the index until we play the song otherwise we will have problems with pausing
                int tempindex = PlayListIndex + 1;
                PlayNewSong(CurrentPlaylist?[tempindex].Path);
            }
        }

        public void PlayPreviousSong()
        {
            if (PlayListIndex > 0)
            {
                // We dont want to adjust the index until we play the song otherwise we will have problems with pausing
                int tempindex = PlayListIndex - 1;
                PlayNewSong(CurrentPlaylist?[tempindex].Path);
            }
        }

        public void PlayRandomSong()
        {

        }

        public void ResumeSong()
        {
            IsPlayingSong = true;
            MusicPlayerStatus = PlayerStatus.Playing;
            SoundOut?.Resume();
        }

        public void PauseSong()
        {
            HowPlayerStopped = PlayerStopped.UserPaused;
            MusicPlayerStatus = PlayerStatus.Paused;
            IsPlayingSong = false;
            SoundOut?.Pause();
        }

        public void StopSong()
        {
            HowPlayerStopped = PlayerStopped.UserStopped;
            MusicPlayerStatus = PlayerStatus.Stopped;
            IsPlayingSong = false;
            SoundOut?.Stop();
        }

        public void Cleanup()
        {
            SoundOut.Stopped -= SoundOut_Stopped;
        }

        public void NotifyProperyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        private int GetPlaylistIndexFromPath(string path)
        {
            int returnVal = -1;

            for (int i = 0; i < CurrentPlaylist.Count; i++)
            {
                if (path == CurrentPlaylist[i].Path)
                {
                    returnVal = i;
                    break;
                }
            }

            return returnVal;
        }

        private void SetSongAsPlayed(int playlistindex)
        {
            for (int i = 0; i < CurrentPlaylist.Count; i++)
            {
                if (i == playlistindex)
                    CurrentPlaylist[i].IsBeingPlayed = true;
                else
                    CurrentPlaylist[i].IsBeingPlayed = false;
            }
        }
    }
}
