using System;
using System.IO;
using System.Threading;
using System.Text;
using System.IO.Compression;

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
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
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
