using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleMusicPlayer.Models;

namespace SimpleMusicPlayer.Services
{
    public struct ID3v2Frame
    {
        /// <summary>
        /// The Frame ID is 4 characters to describe what the frame will be
        /// Items begining with X,Y,Z are experimental
        /// </summary>
        public string FrameID;

        /// <summary>
        /// The size of the data excluding header aka entiresize - 10
        /// </summary>
        public int Size;

        /// <summary>
        /// Two Bytes to indicate the flags.
        /// </summary>
        public byte[] Flags;

        /// <summary>
        /// The data of the frame.
        /// </summary>
        public byte[] Data;

    }

    public class MusicTagReaderService
    {
        private static ID3v2Frame ReadID3v2Frame(FileStream stream)
        {
            ID3v2Frame readframe = new ID3v2Frame();

            // Get the type of frame it is
            byte[] name = new byte[4];
            stream.Read(name, 0, 4);
            readframe.FrameID = Encoding.UTF8.GetString(name);

            // Get the size of the frame
            byte[] size = new byte[4];
            stream.Read(size, 0, 4);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(size);

            readframe.Size = BitConverter.ToInt32(size, 0);

            // Get the flags
            byte[] flags = new byte[2];
            stream.Read(flags, 0, 2);
            readframe.Flags = flags;

            byte[] data = new byte[readframe.Size];
            stream.Read(data, 0, readframe.Size);
            readframe.Data = data;
            string test = Encoding.UTF8.GetString(data).Trim(new char[] { ' ', '\0' });

            switch(readframe.FrameID)
            {
                // Lead Performer
                case "TPE1":
                    break;
                // Band
                case "TPE2":
                    break;
                // Conductor
                case "TPE3":
                    break;
                // Remixed by
                case "TPE4":
                    break;
                // Title/songname
                case "TIT2":
                    break;
                // Title Year
                case "TYER":
                    break;
                // Title Album
                case "TALB":
                    break;
            }

            return readframe;
        }

        public static SongInfo ReadSong(string path)
        {
            // string test = @"E:\Music\Ace of Base\Flowers\17 - Cruel Summer.mp3";
            // path = test;

            bool containsid3v2 = false;            

            // This is a simple ID3v1 Tag Reader
            SongInfo newInfo = new SongInfo();

            using (FileStream id3v2stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                id3v2stream.Seek(0, SeekOrigin.Begin);
                byte[] id3v2check = new byte[3];
                id3v2stream.Read(id3v2check, 0, 3);

                if (Encoding.UTF8.GetString(id3v2check) == "ID3")
                {
                    // TODO Should compare filelength to size given for better checking
                    containsid3v2 = true;

                    byte[] id3v2version = new byte[2];
                    id3v2stream.Read(id3v2version, 0, 2);
                    int major = id3v2version[0];
                    int minor = id3v2version[1];

                    // a - Unsynchronisation - Bit 7 indicates wheter or not unsynchronisation is used
                    // b - Extended Header - Bit 6 indicates whether or not the header is followed by a extended header.
                    // c - Experimental indictor - Bit 5 should be used as an experimental indicator
                    byte[] id3v2flag = new byte[1];
                    id3v2stream.Read(id3v2flag, 0, 1);

                    byte[] id3v2size = new byte[4];
                    id3v2stream.Read(id3v2size, 0, 4);

                    //if (BitConverter.IsLittleEndian)
                        //Array.Reverse(id3v2size);

                    // We AND all the bytes with 0111 1111 1111 1111 because the size in id3v2 the first bit is ignored so we make sure that value is 0
                    int firstbyte = id3v2size[0] & 0x7F;
                    int secondbyte = id3v2size[1] & 0x7F;
                    int thirdbyte = id3v2size[2] & 0x7F;
                    int fourthbyte = id3v2size[3] & 0x7F;

                    int test = fourthbyte << 1;

                    // Since we are ignoreing the first bit it gets a bit weird but not to bad.
                    // We just need to slide each byte to their corisponding spots - 1
                    int tagLength = fourthbyte + (thirdbyte << 7) + (secondbyte << 14) + (firstbyte << 21);                    

                    while (id3v2stream.Position < tagLength)
                    {
                        ID3v2Frame frame = ReadID3v2Frame(id3v2stream);
                        switch(frame.FrameID)
                        {
                            // Lead Performer
                            case "TPE1":
                                newInfo.Artist = Encoding.UTF8.GetString(frame.Data).Trim(new char[] { ' ', '\0' });
                                break;
                            // Band
                            case "TPE2":
                                break;
                            // Conductor
                            case "TPE3":
                                break;
                            // Remixed by
                            case "TPE4":
                                break;
                            // Title/songname
                            case "TIT2":
                                newInfo.SongTitle = Encoding.UTF8.GetString(frame.Data).Trim(new char[] { ' ', '\0' });
                                break;
                            // Title Year
                            case "TYER":
                                newInfo.Year = BitConverter.ToInt32(frame.Data, 0);
                                break;
                            // Title Album
                            case "TALB":
                                newInfo.Album = Encoding.UTF8.GetString(frame.Data).Trim(new char[] { ' ', '\0' });
                                break;
                        }
                    }
                }
            }

            if (!containsid3v2)
            {
                // Logic to read a ID3v1 tag
                using (FileStream id3v1stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {                   
                    id3v1stream.Seek(-128, SeekOrigin.End);
                    // fs.Seek(fs.Length - 128, SeekOrigin.Begin);
                    // char c = (char)fs.ReadByte();
                    byte[] id3v1check = new byte[3];
                    id3v1stream.Read(id3v1check, 0, 3);

                    if (Encoding.UTF8.GetString(id3v1check) == "TAG")
                    {
                        byte[] songtitle = new byte[30];
                        id3v1stream.Read(songtitle, 0, 30);
                        newInfo.SongTitle = Encoding.UTF8.GetString(songtitle).Trim(new char[] { ' ', '\0' });

                        byte[] artist = new byte[30];
                        id3v1stream.Read(artist, 0, 30);
                        newInfo.Artist = Encoding.UTF8.GetString(artist).Trim(new char[] { ' ', '\0' });

                        byte[] album = new byte[30];
                        id3v1stream.Read(album, 0, 30);
                        newInfo.Album = Encoding.UTF8.GetString(album).Trim(new char[] { ' ', '\0' });

                        byte[] year = new byte[4];
                        id3v1stream.Read(year, 0, 4);
                        int formatyear = BitConverter.ToInt32(year, 0);

                        byte[] comment = new byte[30];
                        id3v1stream.Read(comment, 0, 30);
                        newInfo.Comment = Encoding.UTF8.GetString(comment);

                        byte[] genre = new byte[1];
                        id3v1stream.Read(genre, 0, 1);
                        newInfo.Genre = Encoding.UTF8.GetString(genre);
                    }
                    else
                    {
                        newInfo.SongTitle = path;
                        newInfo.Artist = string.Empty;
                        newInfo.Album = string.Empty;
                    }
                }
            }

            return newInfo;
        }        
    }


/*      ID3v2 Frames and meanings from id3.org/id3v2.3.0
4.20    AENC    [[#sec4.20|Audio encryption]]
4.15    APIC    [#sec4.15 Attached picture]
4.11    COMM    [#sec4.11 Comments]
4.25    COMR    [#sec4.25 Commercial frame]
4.26    ENCR    [#sec4.26 Encryption method registration]
4.13    EQUA    [#sec4.13 Equalization]
4.6     ETCO    [#sec4.6 Event timing codes]
4.16    GEOB    [#sec4.16 General encapsulated object]
4.27    GRID    [#sec4.27 Group identification registration]
4.4     IPLS    [#sec4.4 Involved people list]
4.21    LINK    [#sec4.21 Linked information]
4.5     MCDI    [#sec4.5 Music CD identifier]
4.7     MLLT    [#sec4.7 MPEG location lookup table]
4.24    OWNE    [#sec4.24 Ownership frame]
4.28    PRIV    [#sec4.28 Private frame]
4.17    PCNT    [#sec4.17 Play counter]
4.18    POPM    [#sec4.18 Popularimeter]
4.22    POSS    [#sec4.22 Position synchronisation frame]
4.19    RBUF    [#sec4.19 Recommended buffer size]
4.12    RVAD    [#sec4.12 Relative volume adjustment]
4.14    RVRB    [#sec4.14 Reverb]
4.10    SYLT    [#sec4.10 Synchronized lyric/text]
4.8     SYTC    [#sec4.8 Synchronized tempo codes]
4.2.1   TALB    [#TALB Album/Movie/Show title]
4.2.1   TBPM    [#TBPM BPM (beats per minute)]
4.2.1   TCOM    [#TCOM Composer]
4.2.1   TCON    [#TCON Content type]
4.2.1   TCOP    [#TCOP Copyright message]
4.2.1   TDAT    [#TDAT Date]
4.2.1   TDLY    [#TDLY Playlist delay]
4.2.1   TENC    [#TENC Encoded by]
4.2.1   TEXT    [#TEXT Lyricist/Text writer]
4.2.1   TFLT    [#TFLT File type]
4.2.1   TIME    [#TIME Time]
4.2.1   TIT1    [#TIT1 Content group description]
4.2.1   TIT2    [#TIT2 Title/songname/content description]
4.2.1   TIT3    [#TIT3 Subtitle/Description refinement]
4.2.1   TKEY    [#TKEY Initial key]
4.2.1   TLAN    [#TLAN Language(s)]
4.2.1   TLEN    [#TLEN Length]
4.2.1   TMED    [#TMED Media type]
4.2.1   TOAL    [#TOAL Original album/movie/show title]
4.2.1   TOFN    [#TOFN Original filename]
4.2.1   TOLY    [#TOLY Original lyricist(s)/text writer(s)]
4.2.1   TOPE    [#TOPE Original artist(s)/performer(s)]
4.2.1   TORY    [#TORY Original release year]
4.2.1   TOWN    [#TOWN File owner/licensee]
4.2.1   TPE1    [#TPE1 Lead performer(s)/Soloist(s)]
4.2.1   TPE2    [#TPE2 Band/orchestra/accompaniment]
4.2.1   TPE3    [#TPE3 Conductor/performer refinement]
4.2.1   TPE4    [#TPE4 Interpreted, remixed, or otherwise modified by]
4.2.1   TPOS    [#TPOS Part of a set]
4.2.1   TPUB    [#TPUB Publisher]
4.2.1   TRCK    [#TRCK Track number/Position in set]
4.2.1   TRDA    [#TRDA Recording dates]
4.2.1   TRSN    [#TRSN Internet radio station name]
4.2.1   TRSO    [#TRSO Internet radio station owner]
4.2.1   TSIZ    [#TSIZ Size]
4.2.1   TSRC    [#TSRC ISRC (international standard recording code)]
4.2.1   TSSE    [#TSEE Software/Hardware and settings used for encoding]
4.2.1   TYER    [#TYER Year]
4.2.2   TXXX    [#TXXX User defined text information frame]
4.1     UFID    [#sec4.1 Unique file identifier]
4.23    USER    [#sec4.23 Terms of use]
4.9     USLT    [#sec4.9 Unsychronized lyric/text transcription]
4.3.1   WCOM    [#WCOM Commercial information]
4.3.1   WCOP    [#WCOP Copyright/Legal information]
4.3.1   WOAF    [#WOAF Official audio file webpage]
4.3.1   WOAR    [#WOAR Official artist/performer webpage]
4.3.1   WOAS    [#WOAS Official audio source webpage]
4.3.1   WORS    [#WORS Official internet radio station homepage]
4.3.1   WPAY    [#WPAY Payment]
4.3.1   WPUB    [#WPUB Publishers official webpage]
4.3.2   WXXX    [#WXXX User defined URL link frame]
*/
}
