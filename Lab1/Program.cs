using System;

namespace Lab1
{    
    class Program
    {
        static void Main(string[] args)
        {
            YourFile files = new YourFile();
            bool play = true, check = true;

            while (play)
            {
                Console.WriteLine("Choose the number of the action you want to perform...");
                Console.WriteLine("0 -- Exit the program...\n\n");
                Console.WriteLine("Menu:");
                Console.WriteLine("1 -- Create the file");
                Console.WriteLine("2 -- Write data to a file");
                Console.WriteLine("3 -- Read data from a file");
                Console.WriteLine("4 -- Delete the file");
                Console.WriteLine("5 -- Get information about the file");
                Console.WriteLine("6 -- Move file"); 
                Console.WriteLine("7 -- Copying file");
                Console.WriteLine("8 -- Add data to file");
                Console.WriteLine("9 -- Read data from a binary file");
                Console.WriteLine("10 -- Write data to a binary file");
                Console.WriteLine("11 -- Compress");
                Console.WriteLine("12 -- Decompress");

                Console.Write("\n--> ");
                string action = Console.ReadLine();
                check = true;

                while (check)
                { 
                    switch (action)
                    {
                       
                        case "0": Console.Clear(); Console.Write("Good bye)))\n"); play = check = false; break;
                        case "1": Console.Clear(); files.CreatFile(); check = false; break;
                        case "2": Console.Clear(); files.WriteInFile(); check = false; break;
                        case "3": Console.Clear(); files.ReadFile(); check = false; break;
                        case "4": Console.Clear(); files.DeleteFile(); check = false; break;
                        case "5": Console.Clear(); files.FileInformation(); check = false; break;
                        case "6": Console.Clear(); files.MoveFile(); check = false; break;
                        case "7": Console.Clear(); files.CopyingFile(); check = false; break;
                        case "8": Console.Clear(); files.AddToFile(); check = false; break;
                        case "9": Console.Clear(); files.BinRead(); check = false; break;
                        case "10": Console.Clear(); files.BinWrite(); check = false; break;
                        case "11": Console.Clear(); files.Compress(); check = false; break;
                        case "12": Console.Clear(); files.Decompress(); check = false; break;
                        default: 
                            Console.WriteLine("Error! Enter the action number!");
                            Console.Write("\n--> ");
                            action = Console.ReadLine(); 
                            break;
                    }
                }
            }
        }
    }
}
