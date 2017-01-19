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
        UserStopped
    };

    public class MusicPlayerService
    {
        public PlayerStopped HowPlayerStopped = PlayerStopped.SongOver;

        public ObservableCollection<Song> CurrentPlaylist { get; set; } = new ObservableCollection<Song>();

        public string CurrentSong { get; set; } = string.Empty;

        public int PlayListIndex { get; set; } = 0;

        private float volume;
        public float Volume
        {
            get { return volume; }

            set
            {
                volume = value;

                // The slider scale is 0-100 but the library scale is 0-1 so we need to * .01 to get the correct value.
                if (SoundOut != null)
                    SoundOut.Volume = value * 0.01f;
            }
        }

        public ISoundOut SoundOut;
        public IWaveSource WaveSource;        

        public MusicPlayerService()
        {
            Volume = 50f;

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

                PlaySong(CurrentPlaylist[PlayListIndex].Path);
            }

            // User may have stopped it so we reset to song over.
            HowPlayerStopped = PlayerStopped.SongOver;
        }

        public void PlaySong(string path = "")
        {
            string songtoplay = path;

            if (string.IsNullOrEmpty(path))
                songtoplay = CurrentPlaylist[0].Path;

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
            SoundOut.Play();
        }

        public void PauseSong()
        {
            SoundOut?.Pause();            
        }

        public void StopSong()
        {
            SoundOut?.Stop();            
        }

        public void Cleanup()
        {
            SoundOut.Stopped -= SoundOut_Stopped;
        }
    }
}
