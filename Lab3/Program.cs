using System;
using System.Threading;

namespace Lab3
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            Service1 myService = new Service1();
            myService.OnDebug();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
