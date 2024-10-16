using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using Timer = System.Timers.Timer;

namespace csutl
{
    internal class DocumentWatcher
    {
        FileSystemWatcher watcher;
        Timer timer;

        string Path => this.watcher != null ? this.watcher.Path : string.Empty;
        string FullFileName { get; set; } = String.Empty;
        double Delay { get; set; } = 2000;

        internal event Action OnFileChanged;

        internal void Start(string file)
        {
            var path = System.IO.Path.GetDirectoryName(file);
            if (this.Path != path)
            {
                this.Stop();
                if (Directory.Exists(path))
                {
                    this.watcher = new FileSystemWatcher() { Path = path };
                    this.watcher.Changed += this.Watcher_Changed;
                    this.watcher.Created += this.Watcher_Changed;
                    this.watcher.Deleted += this.Watcher_Changed;
                    this.watcher.Renamed += this.Watcher_Changed;

                    this.watcher.EnableRaisingEvents = true;
                }
            }
            this.FullFileName = file;
        }

        internal void Stop()
        {
            this.DisposeTimer();

            if (this.watcher != null)
            {
                this.watcher.EnableRaisingEvents = false;
                this.watcher.Changed -= this.Watcher_Changed;
                this.watcher.Created -= this.Watcher_Changed;
                this.watcher.Deleted -= this.Watcher_Changed;
                this.watcher.Renamed -= this.Watcher_Changed;
                this.watcher.Dispose();
                this.watcher = null;
                this.FullFileName = string.Empty;
            }
        }

        void DisposeTimer()
        {
            if (this.timer != null)
            {
                this.timer.Elapsed -= this.WatcherTimer_Elapsed;
                this.timer.Dispose();
                this.timer = null;
            }
        }

        void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (this.FullFileName == e.FullPath)
            {
                Debug.WriteLine($"Watcher_Changed: {e.ChangeType}, {e.FullPath}");
                if (this.timer == null)
                {
                    this.timer = new Timer()
                    {
                        Interval = this.Delay,
                        AutoReset = false
                    };
                    this.timer.Elapsed += this.WatcherTimer_Elapsed;
                }
                else
                {
                    this.timer.Stop();
                }

                this.timer.Start();
            }
        }
        private void WatcherTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine($"WatcherTimer_Elapsed");
            this.DisposeTimer();
            OnFileChanged?.Invoke();
        }
    }
}
