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

            for (int i = 0; i < 6; i++)
            {
                int index = i;
                tasks[i] = Task.Run(() => {
                    var count = Server.GetCount();
                    Console.WriteLine($"Task {index}: \"Current count = {count}\"");
                });
            }

            for (int i = 6; i < tasks.Length; i++)
            {
                int index = i;
                tasks[i] = Task.Run(() => {
                    Server.AddToCount(index);
                    Console.WriteLine($"Task {index}: \"Add {index} to count\"");
                });
            }

            await Task.WhenAll(tasks);

            Console.ReadKey();
            Server.Destroy();
        }
    }
}
