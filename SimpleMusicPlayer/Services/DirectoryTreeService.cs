﻿using SimpleMusicPlayer.Models.FileTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMusicPlayer.Services
{
    public class DirectoryTreeService
    {
        public static async Task<ObservableCollection<Item>> ReadDirectoriesAsync(IEnumerable paths)
        {
            ObservableCollection<Item> returndirs = new ObservableCollection<Item>();

            foreach (DirectoryItem path in paths)
            {
                ObservableCollection<Item> temp = new ObservableCollection<Item>();
                temp = await Task.Run(() => GetFiles(path.Path)).ConfigureAwait(false);

                if (Directory.Exists(path.Path))
                {
                    DirectoryInfo pdif = new DirectoryInfo(path.Path);
                    DirectoryItem pdi = new DirectoryItem() { Name = pdif.Name, Path = pdif.FullName, Items = temp };

                    returndirs.Add(pdi);
                }
            }

            return returndirs;
        }

        public static ObservableCollection<Item> GetFiles(string path)
        {
            var items = new ObservableCollection<Item>();

            var dirInfo = new DirectoryInfo(path);

            if (Directory.Exists(dirInfo.FullName))
            {
                foreach (var directory in dirInfo.GetDirectories())
                {
                    var item = new DirectoryItem
                    {
                        Name = directory.Name,
                        Path = directory.FullName,
                        Items = GetFiles(directory.FullName)
                    };

                    items.Add(item);
                }

                foreach (var file in dirInfo.GetFiles())
                {
                    // TODO add in smarter music file checker.
                    if (file.FullName.ToUpper().EndsWith(".MP3") ||
                        file.FullName.ToUpper().EndsWith(".WMA"))
                    {
                        var item = new FileItem
                        {
                            Name = file.Name,
                            Path = file.FullName
                        };

                        items.Add(item);
                    }
                }
            }

            return items;
        }
    }
}
