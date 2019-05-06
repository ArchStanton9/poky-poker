using System;
using System.Diagnostics;

namespace PokyPoker.Domain.Tests.Utils
{
    public class ConsoleStopWatch : IDisposable
    {
        private readonly Stopwatch watch;

        public ConsoleStopWatch(Stopwatch watch)
        {
            this.watch = watch;
        }

        public static ConsoleStopWatch StartNew()
        {
            var watch = new Stopwatch();
            var consoleWatch = new ConsoleStopWatch(watch);
            watch.Start();
            return consoleWatch;
        }

        public void Dispose()
        {
            watch.Stop();
            Console.WriteLine(watch.Elapsed);
        }
    }
}
