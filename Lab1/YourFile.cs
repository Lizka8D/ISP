using System;
using System.IO;
using System.Collections;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Text;

namespace Lab1
{
    class YourFile
    {
        //"C:\Users\lizab\Desktop"
        string path;
        string checkPath = @"C:\\?\\?" + @"[A-Za-z]+" + @"\\?\\?" + @".[A-Za-z]+";
        string checkNewPath = @"C:\\?\\?" + @"[A-Za-z]+" + @"\\?\\?";
        ArrayList list = new ArrayList();
        FileInfo fileInf;
        public void CreatFile() 
        {
            while (true)
            {
                Console.WriteLine("Enter the file path:");
                path = Console.ReadLine();

                fileInf = new FileInfo(path);
                if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                {
                    using (var creationStream = fileInf.Create())
                    Console.WriteLine("The file is created!");
                    if (BackToMenu())
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input! Try again.");
                }
            }
        }
        public void WriteInFile()
        {
            while (true)
            {
                Console.WriteLine("Enter the file path:");
                path = Console.ReadLine();

                if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                {

                    Console.WriteLine("Enter the data you want to write to the file:");
                    FileStream fstream = null;

                    try
                    {
                        using (fstream = new FileStream(path, FileMode.Open))
                        {
                            byte[] array = Encoding.Default.GetBytes(Console.ReadLine());
                            fstream.Write(array, 0, array.Length);
                            Console.Clear();
                            Console.WriteLine("The text is written to a file.");

                            if (BackToMenu())
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        if (BackToMenu())
                        {
                            break;
                        }
                    }
                    finally
                    {
                        if (fstream != null)
                        {
                            fstream.Close();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input! Try again.");
                }
            }
        }
        public void ReadFile()
        {
            while (true)
            {
                Console.WriteLine("Enter the file path:");
                path = Console.ReadLine();

                FileStream fstream = null;
                if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                {
                    try
                    {
                        using (fstream = File.OpenRead(path))
                        {
                            byte[] array = new byte[fstream.Length];
                            fstream.Read(array, 0, array.Length);
                            list.Add(Encoding.Default.GetString(array));

                            Console.WriteLine($"Text from the file:");
                            foreach (var o in list)
                            {
                                Console.WriteLine(o);
                            }

                            if (BackToMenu())
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        if (BackToMenu())
                        {
                            break;
                        }
                    }
                    finally
                    {
                        if (fstream != null)
                        {
                            fstream.Close();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input! Try again.");
                }
            }
        }
        public void DeleteFile()
        {
            while (true)
            {
                Console.WriteLine("Enter the file path:");
                path = Console.ReadLine();

                fileInf = new FileInfo(path);
                if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                {
                    if (fileInf.Exists)
                    {
                        fileInf.Delete();
                    }
                    else
                    {
                        Console.WriteLine("Close the file!");
                    }

                    if (BackToMenu())
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input! Try again.");
                }
            }
        }
        public void FileInformation()
        {
            while (true)
            {
                Console.WriteLine("Enter the file path:");
                path = Console.ReadLine();

                fileInf = new FileInfo(path);
                if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                {
                    if (fileInf.Exists)
                    {
                        Console.WriteLine("\nFile name: {0}", fileInf.Name);
                        Console.WriteLine("Creation time: {0}", fileInf.CreationTime);
                        Console.WriteLine("Size: {0}", fileInf.Length);
                    }
                    else
                    {
                        Console.WriteLine("Close the file!");
                    }

                    if (BackToMenu())
                    {
                        break;
                    }
                }
            }
        }
        public void MoveFile()
        {
            while (true)
            {
                Console.WriteLine("Enter the file path:");
                path = Console.ReadLine();

                if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                {
                    Console.WriteLine("Enter the new file path:");
                    string newPath = Console.ReadLine();

                    if (Regex.IsMatch(newPath, checkNewPath, RegexOptions.IgnoreCase))
                    {
                        fileInf = new FileInfo(path);
                        if (fileInf.Exists)
                        {
                            fileInf.MoveTo(newPath);
                        }
                        else
                        {
                            Console.WriteLine("Close the file!");
                        }
                        if (BackToMenu())
                        {
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect input! Try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input! Try again.");
                }
            }
        }
        public void CopyingFile()
        {
            while (true)
            {
                Console.WriteLine("Enter the file path:");
                path = Console.ReadLine();

                if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                {
                    Console.WriteLine("Enter the new file path:");
                    string newPath = Console.ReadLine();

                    if (Regex.IsMatch(path, checkNewPath, RegexOptions.IgnoreCase))
                    {
                        fileInf = new FileInfo(path);
                        if (fileInf.Exists)
                        {
                            fileInf.CopyTo(newPath);
                        }
                        else
                        {
                            Console.WriteLine("Close the file!");
                        }

                        if (BackToMenu())
                        {
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect input! Try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input! Try again.");
                }
            }
        }
        public void AddToFile()
        {
            while (true)
            {
                Console.WriteLine("Enter the file path:");
                path = Console.ReadLine();

                if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                {
                    Console.WriteLine("Enter the data you want to write to the file:");
                    FileStream fstream = null;

                    try
                    {
                        using (fstream = new FileStream(path, FileMode.Append))
                        {
                            byte[] array = Encoding.Default.GetBytes(Console.ReadLine());
                            fstream.Write(array, 0, array.Length);
                            Console.Clear();
                            Console.WriteLine("The text is written to a file.");

                            if (BackToMenu())
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        if (BackToMenu())
                        {
                            break;
                        }
                    }
                    finally
                    {
                        if (fstream != null)
                        {
                            fstream.Close();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input! Try again.");
                }

            }
        }
        public void BinRead()
        {
            while (true)
            {
                Console.WriteLine("Enter the file path:");
                path = Console.ReadLine();

                if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                {
                    try
                    {
                        using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                        {
                            while (reader.PeekChar() > -1)
                            {
                                list.Add(reader.ReadString());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    if (BackToMenu())
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input! Try again.");
                }
            }
        }
        public void BinWrite()
        {
            while (true)
            {
                Console.WriteLine("Enter the file path:");
                path = Console.ReadLine();

                if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                {
                    try
                    {
                        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
                        {
                            foreach (var o in list)
                            {
                                writer.Write(o as string);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    if (BackToMenu())
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input! Try again.");
                }
            }
        }
        public void Compress()
        {
            while (true)
            {
                Console.WriteLine("Enter the source file:");
                string sourceFile = Console.ReadLine();

                if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                {
                    Console.WriteLine("Enter the compressed file:");
                    string compressedFile = Console.ReadLine();

                    if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                    {
                        // поток для чтения исходного файла
                        using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
                        {
                            // поток для записи сжатого файла
                            using (FileStream targetStream = File.Create(compressedFile))
                            {
                                // поток архивации
                                using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                                {
                                    sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
                                    Console.WriteLine("MyFile compression {0} completed. Original size: {1}  Compressed size: {2}.",
                                        sourceFile, sourceStream.Length.ToString(), targetStream.Length.ToString());
                                }
                            }
                        }
                        if (BackToMenu())
                        {
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect input! Try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input! Try again.");
                }
            }
        }

        public void Decompress()
        {
            while (true)
            {
                Console.WriteLine("Enter the compressed file:");
                string compressedFile = Console.ReadLine();

                if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                {
                    Console.WriteLine("Enter the target file:");
                    string targetFile = Console.ReadLine();

                    if (Regex.IsMatch(path, checkPath, RegexOptions.IgnoreCase))
                    {
                        FileInfo fileInfo = new FileInfo(compressedFile);
                        if (fileInfo.Exists)
                        {
                            // поток для чтения из сжатого файла
                            using (FileStream sourceStream = new FileStream(compressedFile, FileMode.Open))
                            {
                                // поток для записи восстановленного файла
                                using (FileStream targetStream = File.Create(targetFile))
                                {
                                    // поток разархивации
                                    using (GZipStream decompressionStream =
                                        new GZipStream(sourceStream, CompressionMode.Decompress))
                                    {
                                        decompressionStream.CopyTo(targetStream);
                                        Console.WriteLine("MyFile restored: {0}", targetFile);
                                    }
                                }
                            }
                        }
                        else Console.WriteLine("Error. Compressed file don't exist.");

                        if (BackToMenu())
                        {
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect input! Try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input! Try again.");
                }
            }
        }
        private bool BackToMenu()
        {
            Console.WriteLine("\nEnter '1' to return to the menu");
            Console.Write("--> ");
            int answer = Convert.ToInt32(Console.ReadLine());

            if (answer == 1)
            {
                Console.Clear();
                return true;
            }
            else
            {
                Console.WriteLine("Error! Enter the answer!");
                return false;
            }
        }
    }
}
