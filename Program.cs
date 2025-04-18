using System;
using System.Threading;
using System.Threading.Tasks;

namespace Task2
{
    internal static class Server
    {
        private static int count = 0;
        private static readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public static int GetCount()
        {
            rwLock.EnterReadLock();
            try
            {
                return count;
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public static void AddToCount(int value)
        {
            rwLock.EnterWriteLock();
            try
            {
                count += value;
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public static void Destroy()
        {
            rwLock.Dispose();
        }
    }

    internal class Program
    {
        public static async Task Main()
        {
            var tasks = new Task[8];

            for (int i = 0; i < 6; ++i)
            {
                tasks[i] = Task.Run(() => {
                    Console.WriteLine($"Read count = {Server.GetCount()}");
                });
            }

            for (int i = 6; i < tasks.Length; ++i)
            {
                int index = i;
                tasks[i] = Task.Run(() => {
                    Server.AddToCount(index);
                    Console.WriteLine($"Task {index}: \"Add {index} to count\"");
                });
            }

            await Task.WhenAll(tasks);
            // Иногда WriteLine не успевает за изменениями в Server. 
            // Либо оставить так, ибо на фактическом значении count оно не влияет, либо вынести WriteLine внутрь методов сервера.

            Console.ReadKey();
            Server.Destroy();
            
            for (int i = 0; i < tasks.Length; ++i)
                tasks[i].Dispose();
        }
    }
}
