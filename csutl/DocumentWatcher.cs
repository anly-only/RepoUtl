using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Timer = System.Timers.Timer;

namespace csutl
{
    internal class DocumentWatcher
    {
        FileSystemWatcher watcher;
        Timer timer;

        string Path => watcher != null ? watcher.Path : string.Empty;
        string FullFileName { get; set; } = String.Empty;
        double Delay { get; set; } = 2000;

        internal event Action OnFileChanged;

        internal void Start(string file)
        {
            var path = System.IO.Path.GetDirectoryName(file);
            if (Path != path)
            {
                Stop();
                if (Directory.Exists(path))
                {
                    watcher = new FileSystemWatcher() { Path = path };
                    watcher.Changed += this.Watcher_Changed;
                    watcher.Created += this.Watcher_Changed;
                    watcher.Deleted += this.Watcher_Changed;
                    watcher.Renamed += this.Watcher_Changed;

                    watcher.EnableRaisingEvents = true;
                }
            }
            FullFileName = file;
        }

        internal void Stop()
        {
            DisposeTimer();

            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Changed -= Watcher_Changed;
                watcher.Created -= this.Watcher_Changed;
                watcher.Deleted -= this.Watcher_Changed;
                watcher.Renamed -= this.Watcher_Changed;
                watcher.Dispose();
                watcher = null;
                FullFileName = string.Empty;
            }
        }

        void DisposeTimer()
        {
            if (timer != null)
            {
                timer.Elapsed -= WatcherTimer_Elapsed;
                timer.Dispose();
                timer = null;
            }
        }

        void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (FullFileName == e.FullPath)
            {
                Debug.WriteLine($"Watcher_Changed: {e.ChangeType}, {e.FullPath}");
                if (timer == null)
                {
                    timer = new Timer()
                    {
                        Interval = Delay,
                        AutoReset = false
                    };
                    timer.Elapsed += WatcherTimer_Elapsed;
                }
                else
                {
                    timer.Stop();
                }

                timer.Start();
            }
        }
        private void WatcherTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine($"WatcherTimer_Elapsed");
            DisposeTimer();
            OnFileChanged?.Invoke();
        }
    }
}
