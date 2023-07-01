
using LogComponent;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger1 = StartLogger();

            for (int i = 0; i < 15; i++)
            {
                logger1.WriteToLog($"Number with Flush: {i}");
                Thread.Sleep(50);
            }

            logger1.StopWithFlush();

            var logger2 = StartLogger();
            for (int i = 50; i > 0; i--)
            {
                logger2.WriteToLog($"Number with No flush: {i}");
                Thread.Sleep(20);
            }

            logger2.StopWithoutFlush();

            Console.ReadLine();
        }

        static void ExceptionHandler(Task task)
        {
            var exception = task.Exception;
            Console.WriteLine(exception.InnerException.Message);
        }

        private static IAsyncLogger StartLogger()
        {
            IAsyncLogger logger = new AsyncLogger(new SystemClock());
            var task = new Task(logger.MainLoop);
            task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();

            return logger;
        }
    }
}