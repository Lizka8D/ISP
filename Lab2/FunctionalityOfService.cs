using System;
using System.Text;
using System.IO;
using System.IO.Compression;
namespace Lab2
{
    class FunctionalityOfService
    {
        public string Source { get; set; }

        public string Target { get; set; }

        public string FileName { get; set; }

        public FileInfo Info { get; set; }
        public FunctionalityOfService(string source, string target, string fileName)
        {
            this.Source = source;
            this.Target = target;
            this.FileName = fileName;
            this.Info = new FileInfo(source + fileName);
        }
        public void MainFunction()
        {
            string passPhrase = "TestPassphrase";
            string saltValue = "TestSaltValue";
            string hashAlgorithm = "SHA256";
            int passwordIterations = 2;
            string initVector = "!1A3g2D4s9K556g7";
            int keySize = 256;

            string cipherText = SymmetricEncryption.Encrypt
            (
                ReadFile(Source + FileName),
                passPhrase,
                saltValue,
                hashAlgorithm,
                passwordIterations,
                initVector,
                keySize
            );

            OverwriteFile(Source + FileName, cipherText);

            string path = this.CreateFolders();
            string newFileName = $"\\{Info.CreationTime.Year}_{Info.CreationTime.Month}_{Info.CreationTime.Day}_{Info.CreationTime.Hour}" +
                $"_{Info.CreationTime.Minute}_{Info.CreationTime.Second}.txt";

            File.Copy(Source + FileName, Source + newFileName, true);
            File.Delete(Source + FileName);

            Compress(Source + newFileName, newFileName);
            File.Delete(Source + newFileName);

            File.Copy(Source + FileName, path + FileName, true);
            File.Delete(Source + FileName);

            string oldFileName = FileName;
            Decompress(path + FileName);
            File.Delete(path + oldFileName);

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

            OverwriteFile(path + FileName, plainText);
        }
        private string ReadFile(string path)
        {
            FileStream fstream = null;
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
            FileStream fstream = null;
            using (fstream = new FileStream(path, FileMode.Truncate))
            {
                byte[] array = Encoding.Default.GetBytes(cipherText);
                fstream.Write(array, 0, array.Length);
            }
        }
        private void Compress(string sourceFile, string newFileName)
        {
            string compressedFile = sourceFile.Replace(".txt", ".rar");
            FileName = newFileName.Replace(".txt", ".rar");

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
        private void Decompress(string compressedFile)
        {
            string targetFile = compressedFile.Replace(".rar", ".txt");
            FileName = FileName.Replace(".rar", ".txt");

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
                using (StreamWriter writer = new StreamWriter("C:\\Users\\lizab\\Desktop\\Study\\Progs\\3_semester\\C#\\FileLog.txt", true))
                {
                    writer.WriteLine("Error. Compressed file don't exist.");
                    writer.Close();
                }
            }

        }
        public string CreateFolders()
        {
            string year = Info.CreationTime.Year.ToString();
            string month = Info.CreationTime.Month.ToString();
            string day = Info.CreationTime.Day.ToString();
            string path = $"{Target}\\{year}\\{month}\\{day}";
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
