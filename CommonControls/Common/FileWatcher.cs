﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

namespace CommonControls.Common
{
    public class FileWatcher
    {
        public delegate void FileChangedDelegate(string path);
        public delegate void FileDeletedDelegate(string path);

        private string _path;
        private readonly FileSystemWatcher _watcher;

        public event FileChangedDelegate FileChanged;
        public event FileDeletedDelegate FileDeleted;

        public FileWatcher(string path)
        {
            _path = path;

            var watcher = new FileSystemWatcher(System.IO.Path.GetDirectoryName(_path));
            watcher.NotifyFilter = NotifyFilters.Attributes
                                   | NotifyFilters.CreationTime
                                   | NotifyFilters.DirectoryName
                                   | NotifyFilters.FileName
                                   | NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite
                                   | NotifyFilters.Security
                                   | NotifyFilters.Size;
            watcher.Changed += WatcherOnChanged;
            watcher.Deleted += WatcherOnDeleted;
            watcher.Filter = System.IO.Path.GetFileName(_path);
            watcher.EnableRaisingEvents = true;

            _watcher = watcher;
        }

        ~FileWatcher()
        {
            _watcher.Changed -= WatcherOnChanged;
            _watcher.Deleted -= WatcherOnDeleted;
            _watcher.Dispose();
        }


        private void WatcherOnDeleted(object sender, FileSystemEventArgs e)
        {
            FileDeleted?.Invoke(_path);
        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            FileChanged?.Invoke(_path);
        }

        public string Path
        {
            get => _path;
            set => _path = value;
        }
    }
}
