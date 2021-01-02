using System;
using System.ServiceProcess;
using System.IO;
using System.Threading.Tasks;

namespace Lab3
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static async Task void Main()
        {
            Service1 myService = new Service1();
            
            try
            {
                ServiceBase.Run(myService);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exceptions.txt"), true))
                {
                    sw.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Exception: {ex.Message}");
                }
            }
        }
    }
}
