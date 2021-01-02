using System;
using System.IO;
using System.Threading;
using System.Text;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Lab3
{
    class Logger
    {
        private FileSystemWatcher watcher;
        private readonly Timer timer = new Timer();
        private readonly StringBuilder messages = new StringBuilder();
        private readonly List<string> createdFiles = new List<string>();
        private readonly Options options;
        private readonly object obj = new object();
        bool enabled = true;
        public Logger(Options options)
        {
            this.options = options;

            if (!Directory.Exists(this.options.SourcePath))
            {
                Directory.CreateDirectory(this.options.SourcePath);
            }

            if (!Directory.Exists(this.options.TargetPath))
            {
                Directory.CreateDirectory(this.options.TargetPath);
            }

            watcher = new FileSystemWatcher(this.options.SourcePath);

            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;
        }

        public void Start()
        {
            WriteToFileAsync($"Service was started at {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n");
            watcher.EnableRaisingEvents = true;
            timer.Start();
        }
        public void Stop()
        {
            timer.Stop();
            watcher.EnableRaisingEvents = false;
            messages.Clear();
            WriteToFileAsync($"Service was stopped at {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n");
        }
        private async void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            if (!Directory.Exists(options.SourcePath))
            {
                await Task.Run(() => Directory.CreateDirectory(options.SourcePath));
                watcher = new FileSystemWatcher(options.SourcePath);
                watcher.Deleted += Watcher_Deleted;
                watcher.Created += Watcher_Created;
                watcher.Changed += Watcher_Changed;
                watcher.Renamed += Watcher_Renamed;
                watcher.EnableRaisingEvents = true;
            }
            if (!Directory.Exists(options.TargetPath))
            {
                await Task.Run(() => Directory.CreateDirectory(options.TargetPath));
            }
            if (messages.Length > 0)
            {
                await WriteToFileAsync(messages.ToString());
                messages.Clear();
            }
            if (createdFiles.Count == 0) return;
            watcher.EnableRaisingEvents = false;
            try
            {
                string createdFile = createdFiles[0];
                await WriteToFileAsync(createdFile);
                createdFiles.RemoveAt(0);
                FileInfo fileInfo = new FileInfo(createdFile);
                string newFileName = $"Sales_{fileInfo.CreationTime:dd_MM_yyyy_HH_mm_ss}";
                newFileName += fileInfo.Extension;
                string newFilePath = Path.Combine(options.SourcePath, newFileName);
                string newTargetPath = Path.Combine(options.TargetPath, newFileName);
                if (options.ArchiveOptions.NeedToArchive)
                {
                    string temp = newFileName;
                    newFileName += ".zip";
                    newFilePath = Path.Combine(options.SourcePath, newFileName);
                    newTargetPath = Path.Combine(options.TargetPath, newFileName);
                    int counter = 1;
                    while (File.Exists(newFilePath) || File.Exists(newTargetPath))
                    {
                        newFileName = "(" + counter.ToString() + ")" + temp + ".gz";
                        newFilePath = Path.Combine(options.SourcePath, newFileName);
                        newTargetPath = Path.Combine(options.TargetPath, newFileName);
                        counter++;
                    }
                    await CompressAsync(createdFile, newFilePath);
                }
                else
                {
                    string temp = newFileName;
                    newFilePath = Path.Combine(options.SourcePath, newFileName);
                    newTargetPath = Path.Combine(options.TargetPath, newFileName);
                    int counter = 1;
                    while (File.Exists(newFilePath) || File.Exists(newTargetPath))
                    {
                        newFileName = "(" + counter.ToString() + ")" + temp;
                        newFilePath = Path.Combine(options.SourcePath, newFileName);
                        newTargetPath = Path.Combine(options.TargetPath, newFileName);
                        counter++;
                    }
                    await Task.Run(() => File.Copy(createdFile, newFilePath));
                }
                if (options.NeedToEncrypt)
                {
                    await Task.Run(() => File.Encrypt(newFilePath));
                }
                await Task.Run(() => File.Move(newFilePath, newTargetPath));
                if (options.NeedToEncrypt)
                {
                    await Task.Run(() => File.Decrypt(newTargetPath));
                }
                string decompressedFilePath = Path.Combine(
                    options.TargetPath,
                    "archive",
                    fileInfo.CreationTime.ToString("yyyy"),
                    fileInfo.CreationTime.ToString("MM"),
                    fileInfo.CreationTime.ToString("dd"));

                if (!Directory.Exists(decompressedFilePath))
                {
                    await Task.Run(() => Directory.CreateDirectory(decompressedFilePath));
                }
                if (options.ArchiveOptions.NeedToArchive)
                {
                    decompressedFilePath = Path.Combine(decompressedFilePath, newFileName.Remove(newFileName.Length - 3, 3));
                    await DecompressAsync(newTargetPath, decompressedFilePath);
                }
                else
                {
                    decompressedFilePath = Path.Combine(decompressedFilePath, newFileName);
                    await Task.Run(() => File.Copy(newTargetPath, decompressedFilePath));
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exceptions.txt"), true))
                {
                    await sw.WriteLineAsync($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Exception: {ex.Message}");
                }
            }
            watcher.EnableRaisingEvents = true;
        }
        // переименование файлов
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = "переименован в " + e.FullPath;
            string filePath = e.OldFullPath;
            AddToMessages(filePath, fileEvent);
        }
        // изменение файлов
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "изменен";
            string filePath = e.FullPath;
            AddToMessages(filePath, fileEvent);
        }
        // создание файлов
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "создан";
            string filePath = e.FullPath;
            AddToMessages(filePath, fileEvent);
        }
        // удаление файлов
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "удален";
            string filePath = e.FullPath;
            AddToMessages(filePath, fileEvent);
        }
        public Task WriteToFileAsync(string message)
        {
            if (!Directory.Exists(options.SourcePath))
            {
                Directory.CreateDirectory(options.SourcePath);
                watcher = new FileSystemWatcher(options.SourcePath);
                watcher.Deleted += Watcher_Deleted;
                watcher.Created += Watcher_Created;
                watcher.Changed += Watcher_Changed;
                watcher.Renamed += Watcher_Renamed;
                watcher.EnableRaisingEvents = true;
            }
            if (!Directory.Exists(options.TargetPath))
            {
                Directory.CreateDirectory(options.TargetPath);
            }
            using (StreamWriter sw = new StreamWriter(options.LogFilePath, true))
            {
                return sw.WriteAsync(message);
            }
        }
        void AddToMessages(string filePath, string fileEvent)
        {
            messages.Append($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} file {filePath} was {fileEvent}\n");
        }
        async Task CompressAsync(string sourceFile, string compressedFile)
        {
            await Task.Run(() =>
            {
                using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open))
                {
                    using (FileStream targetStream = new FileStream(compressedFile, FileMode.OpenOrCreate))
                    {
                        using (GZipStream compressionStream = new GZipStream(targetStream, options.ArchiveOptions.Level))
                        {
                            sourceStream.CopyTo(compressionStream);
                        }
                    }
                }
            });
        }
        async Task DecompressAsync(string compressedFile, string targetFile)
        {
            await Task.Run(() =>
            {
                using (FileStream sourceStream = new FileStream(compressedFile, FileMode.Open))
                {
                    using (FileStream targetStream = new FileStream(targetFile, FileMode.OpenOrCreate))
                    {
                        using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(targetStream);
                        }
                    }
                }
            });
        }
    }
}
