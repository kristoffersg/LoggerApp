using Moq;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using System.Text;

namespace LogComponent.Test
{
    public class AsyncLoggerTest
    {
        private AsyncLogger _asyncLogger;
        private readonly Mock<ISystemClock> _systemClockMock;

        public AsyncLoggerTest()
        {
            _systemClockMock = new Mock<ISystemClock>();
        }

        [Fact]
        public void WriteToLog_CallWithString_WritesToLog()
        {
            // Arrange
            DeleteAllLogs();
            var testText = "This is a test to see if message is written to the log";
            _asyncLogger = new AsyncLogger(_systemClockMock.Object);
            var task = new Task(_asyncLogger.MainLoop);
            task.Start();

            // Act
            _asyncLogger.WriteToLog(testText);
            Thread.Sleep(1000);
            _asyncLogger.StopWithoutFlush();
            Thread.Sleep(1000);

            // Assert
            var directory = new DirectoryInfo(@"C:\LogTest");
            var myFile = directory.GetFiles().First();

            using (var fs = new FileStream(myFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.Default))
            {
                var line = sr.ReadToEnd();
                Assert.Contains(testText, line);
            }
        }

        [Fact]
        public void WriteToLog_CrossMidnight_CreatestwoFiles()
        {
            // Arrange
            DeleteAllLogs();
            _systemClockMock.SetupGet(x => x.GetDateTimeNow).Returns(new DateTime(2024, 02, 03, 23, 55, 33));
            _asyncLogger = new AsyncLogger(_systemClockMock.Object);
            var task = new Task(_asyncLogger.MainLoop);
            task.Start();

            // Act
            _asyncLogger.WriteToLog("1");
            Thread.Sleep(2000);
            _systemClockMock.SetupGet(x => x.GetDateTimeNow).Returns(new DateTime(2024, 02, 04, 00, 00, 11));
            _asyncLogger.WriteToLog("1");
            Thread.Sleep(2000);

            // Assert
            var directory = Directory.EnumerateFiles(@"C:\LogTest");
            var files = directory.Count();
            Assert.Equal(2, files);
        }

        [Fact]
        public void Hejsa()
        {
            // Arrange
            IAsyncLogger logger = new AsyncLogger(new SystemClock());
            var task1 = new Task(logger.MainLoop);
            task1.Start();

            for (int i = 0; i < 15; i++)
            {
                logger.WriteToLog($"Number with Flush: {i}");
                Thread.Sleep(50);
            }

            logger.StopWithFlush();

            IAsyncLogger logger2 = new AsyncLogger(new SystemClock());
            var task2 = new Task(logger2.MainLoop);
            task2.Start();

            for (int i = 50; i > 0; i--)
            {
                logger2.WriteToLog($"Number with No flush: {i}");
                Thread.Sleep(20);
            }

            logger2.StopWithoutFlush();
            Thread.Sleep(3000);

            // Assert
            var directory = new DirectoryInfo(@"C:\LogTest");
            var firstLog = directory.GetFiles()[0];
            var secondLog = directory.GetFiles()[1];

            var firstLogLines = File.ReadAllLines(firstLog.FullName).Count();
            var secondLogLines = File.ReadAllLines(secondLog.FullName).Count();

            Assert.Equal(16, firstLogLines);
            Assert.True(secondLogLines < 50);
        }

        private void DeleteAllLogs()
        {
            var dir = new DirectoryInfo(@"C:\LogTest");
            foreach (var file in dir.GetFiles())
            {
                file.Delete();
            }
        }

        private static void StartLogger(IAsyncLogger logger)
        {
            var task = new Task(logger.MainLoop);
            task.Start();
        }
    }
}