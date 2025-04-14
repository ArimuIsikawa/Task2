using System;
using System.Threading;

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
    }

    internal class Program
    {
        public static void Main()
        {
            // Пример использования
            Thread writerThread1 = new Thread(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Server.AddToCount(1);
                    Console.WriteLine("Writer1: Added 1 to count.");
                    Thread.Sleep(100);
                }
            });

            Thread writerThread2 = new Thread(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Server.AddToCount(1);
                    Console.WriteLine("Writer2: Added 1 to count.");
                    Thread.Sleep(70);
                }
            });

            Thread readerThread1 = new Thread(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"Current count from Reader 1: {Server.GetCount()}");
                    Thread.Sleep(50);
                }
            });

            Thread readerThread2 = new Thread(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"Current count from Reader 2: {Server.GetCount()}");
                    Thread.Sleep(60);
                }
            });

            writerThread1.Start();
            writerThread2.Start();
            readerThread1.Start();
            readerThread2.Start();

            writerThread1.Join();
            writerThread2.Join();
            readerThread1.Join();
            readerThread2.Join();

            Console.ReadKey();
        }
    }
}
