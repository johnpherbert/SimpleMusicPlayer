using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleMusicPlayer.Models;

namespace SimpleMusicPlayer.Services
{
    public class MusicTagReaderService
    {
        public static SongInfo ReadSong(string path)
        {
            // string test = @"E:\Music\Ace of Base\Flowers\17 - Cruel Summer.mp3";
            // path = test;

            // This is a simple ID3v1 Tag Reader
            SongInfo newInfo = new SongInfo();

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(-128, SeekOrigin.End);
                // fs.Seek(fs.Length - 128, SeekOrigin.Begin);
                // char c = (char)fs.ReadByte();
                byte[] id3v1check = new byte[3];
                fs.Read(id3v1check, 0, 3);

                if(Encoding.UTF8.GetString(id3v1check) == "TAG")
                {
                    byte[] songtitle = new byte[30];
                    fs.Read(songtitle, 0, 30);
                    newInfo.SongTitle = Encoding.UTF8.GetString(songtitle).Trim(new char[] { ' ', '\0' });

                    byte[] artist = new byte[30];
                    fs.Read(artist, 0, 30);
                    newInfo.Artist = Encoding.UTF8.GetString(artist).Trim(new char[] { ' ', '\0' });

                    byte[] album = new byte[30];
                    fs.Read(album, 0, 30);
                    newInfo.Album = Encoding.UTF8.GetString(album).Trim(new char[] { ' ', '\0' });

                    byte[] year = new byte[4];
                    fs.Read(year, 0, 4);
                    int formatyear = 0;
                    if (Int32.TryParse(Encoding.UTF8.GetString(year), out formatyear))
                        newInfo.Year = formatyear;
                    else
                        newInfo.Year = 0;

                    byte[] comment = new byte[30];
                    fs.Read(comment, 0, 30);
                    newInfo.Comment = Encoding.UTF8.GetString(comment);

                    byte[] genre = new byte[1];
                    fs.Read(genre, 0, 1);
                    newInfo.Genre = Encoding.UTF8.GetString(genre);
                }
                else
                {
                    newInfo.SongTitle = path;
                    newInfo.Artist = string.Empty;
                    newInfo.Album = string.Empty;
                }
            }

            return newInfo;
        }        
    }
}
