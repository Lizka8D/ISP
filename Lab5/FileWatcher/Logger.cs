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
        private readonly FileSystemWatcher watcher;
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
                await Task.Run(() => Directory.CreateFolders(options.SourcePath));
                watcher = new FileSystemWatcher(options.SourcePath);
                watcher.Deleted += Watcher_Deleted;
                watcher.Created += Watcher_Created;
                watcher.Changed += Watcher_Changed;
                watcher.Renamed += Watcher_Renamed;
                watcher.EnableRaisingEvents = true;
            }
            if (!Directory.Exists(options.TargetPath))
            {
                await Task.Run(() => Directory.CreateFolders(options.TargetPath));
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
                    newFileName += ".gz";
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
                    await Task.Run(() => Directory.CreateFolders(decompressedFilePath));
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
            RecordEntry(fileEvent, filePath);
        }
        // изменение файлов
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "изменен";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        // создание файлов
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "создан";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        // удаление файлов
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "удален";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
         public Task WriteToFileAsync(string message)
        {
            if (!Directory.Exists(options.SourcePath))
            {
                Directory.CreateDirectory(options.SourcePath);
                watcher = new FileSystemWatcher(options.SourcePath);
                watcher.Deleted += OnDeleted;
                watcher.Created += OnCreated;
                watcher.Changed += OnChanged;
                watcher.Renamed += OnRenamed;
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
         void AddToMessages(string filePath, string fileEvent)
        {
            messages.Append($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} file {filePath} was {fileEvent}\n");
        }
        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(this.options.LogFilePath, true))
                {
                    writer.WriteLine(String.Format("{0} файл {1} был {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                    writer.Close();
                }
                if (fileEvent == "изменен")
                {
                    FunctionalityOfService(options.SourcePath.Split((char)92)[filePath.Split((char)92).Length - 1]);
                }
            }
        }
        public void FunctionalityOfService(string FileName)
        {
            FileInfo Info = new FileInfo(options.SourcePath + FileName);
            string newPath = " ";
            string cipherText = " ";

            string passPhrase = "TestPassphrase";
            string saltValue = "TestSaltValue";
            string hashAlgorithm = "SHA256";
            int passwordIterations = 2;
            string initVector = "!1A3g2D4s9K556g7";
            int keySize = 256;

            while (true)
            {
                if (options.NeedToEncrypt)
                {
                    cipherText = SymmetricEncryption.Encrypt
                    (
                        ReadFile(options.SourcePath + FileName),
                        passPhrase,
                        saltValue,
                        hashAlgorithm,
                        passwordIterations,
                        initVector,
                        keySize
                    );

                    OverwriteFile(options.SourcePath + FileName, cipherText);

                    options.NeedToEncrypt = false;
                    options.ArchiveOptions.NeedToCompress = true;

                }
                else
                {
                    string plainText = SymmetricEncryption.Decrypt
                    (
                        cipherText,
                        passPhrase,
                        saltValue,
                        hashAlgorithm,
                        passwordIterations,
                        initVector,
                        keySize
                    );

                    OverwriteFile(newPath + FileName, plainText);

                    break;
                }
                if (options.ArchiveOptions.NeedToCompress)
                {
                    newPath = CreateFolders(Info);
                    string newFileName = $"\\{Info.CreationTime.Year}_{Info.CreationTime.Month}_{Info.CreationTime.Day}_{Info.CreationTime.Hour}" +
                        $"_{Info.CreationTime.Minute}_{Info.CreationTime.Second}.txt";

                    File.Copy(options.SourcePath + FileName, options.SourcePath + newFileName, true);
                    File.Delete(options.SourcePath + FileName);

                    Compress(options.SourcePath + newFileName, newFileName);

                    File.Delete(options.SourcePath + newFileName);

                    options.ArchiveOptions.NeedToCompress = false;
                    options.ArchiveOptions.NeedToMove = true;
                }
                else
                {
                    string oldFileName = FileName;
                    Decompress(newPath + FileName, FileName);
                    File.Delete(newPath + oldFileName);

                    options.NeedToEncrypt = true;
                }
                if (options.ArchiveOptions.NeedToMove)
                {
                    File.Copy(options.SourcePath + FileName, newPath + FileName, true);
                    File.Copy(options.SourcePath + FileName, newPath + "\\Archive" + FileName, true);

                    File.Delete(options.SourcePath + FileName);

                    options.ArchiveOptions.NeedToMove = false;
                    options.ArchiveOptions.NeedToCompress = true;
                }
            }
        }
        private string ReadFile(string path)
        {
            FileStream fstream;
            string textFromFile = " ";
            using (fstream = File.OpenRead(path))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                textFromFile = Encoding.Default.GetString(array);
            }

            return textFromFile;
        }
        private void OverwriteFile(string path, string cipherText)
        {
            FileStream fstream;
            using (fstream = new FileStream(path, FileMode.Truncate))
            {
                byte[] array = Encoding.Default.GetBytes(cipherText);
                fstream.Write(array, 0, array.Length);
            }
        }
        private void Compress(string sourceFile, string newFileName)
        {
            string compressedFile = sourceFile.Replace(".txt", ".rar");
            newFileName.Replace(".txt", ".rar");

            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
            {
                using (FileStream targetStream = File.Create(compressedFile))
                {
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream);
                    }
                }
            }
        }
        private void Decompress(string compressedFile, string FileName)
        {
            string targetFile = compressedFile.Replace(".rar", ".txt");
            FileName.Replace(".rar", ".txt");

            FileInfo fileInfo = new FileInfo(compressedFile);
            if (fileInfo.Exists)
            {
                using (FileStream sourceStream = new FileStream(compressedFile, FileMode.Open))
                {
                    using (FileStream targetStream = File.Create(targetFile))
                    {
                        using (GZipStream decompressionStream =
                            new GZipStream(sourceStream, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(targetStream);
                        }
                    }
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(options.LogFilePath, true))
                {
                    writer.WriteLine("Error. Compressed file don't exist.");
                    writer.Close();
                }
            }

        }
        private string CreateFolders(FileInfo Info)
        {
            string year = Info.CreationTime.Year.ToString();
            string month = Info.CreationTime.Month.ToString();
            string day = Info.CreationTime.Day.ToString();
            string path = $"{options.TargetPath}\\{year}\\{month}\\{day}";
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(path + "\\Archive");
            return path;
        }
    }
}
