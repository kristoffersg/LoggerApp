using System.Diagnostics;
using System.Text;

namespace LogComponent
{
    public class AsyncLogger : IAsyncLogger
    {
        private List<LogLine> _lines = new List<LogLine>();
        private StreamWriter _writer;
        private DateTimeOffset _currentDate;
        private bool _exit;
        private readonly ISystemClock _systemClock;
        private bool _QuitWithFlush = false;

        public AsyncLogger(ISystemClock systemClock)
        {
            _systemClock = systemClock;
        }

        public void StopWithoutFlush()
        {
            _exit = true;
        }

        public void StopWithFlush()
        {
            _QuitWithFlush = true;
        }

        public void WriteToLog(string s)
        {
            _lines.Add(new LogLine() { Text = s, Timestamp = _systemClock.GetDateTimeNow });
        }

        public void MainLoop()
        {
            if (!Directory.Exists(@"C:\LogTest"))
                Directory.CreateDirectory(@"C:\LogTest");

            CreateNewFile();
            _currentDate = _systemClock.GetDateTimeNow;
            _writer.AutoFlush = true;

            while (!_exit)
            {
                if (_lines.Count > 0)
                {
                    int f = 0;
                    List<LogLine> handledLogLines = new List<LogLine>();

                    foreach (LogLine logLine in _lines.ToArray())
                    {
                        f++;

                        if (f > 5)
                            continue;

                        if (!_exit || _QuitWithFlush)
                        {
                            handledLogLines.Add(logLine);

                            StringBuilder stringBuilder = new StringBuilder();

                            CheckDate(stringBuilder);

                            stringBuilder.Append($"{logLine.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff")}");
                            stringBuilder.Append("\t");
                            stringBuilder.Append(logLine.LineText());
                            stringBuilder.Append("\t");

                            stringBuilder.Append(Environment.NewLine);

                            _writer.Write(stringBuilder.ToString());
                        }
                    }

                    for (int y = 0; y < handledLogLines.Count; y++)
                    {
                        _lines.Remove(handledLogLines[y]);
                    }

                    if (_QuitWithFlush == true && _lines.Count == 0)
                        _exit = true;

                    Thread.Sleep(200);
                }
            }
            _writer.Close();
        }

        private void CheckDate(StringBuilder stringBuilder)
        {
            if (_systemClock.GetDateTimeNow.Date.ToUniversalTime() > _currentDate.ToUniversalTime())
            {
                _currentDate = _systemClock.GetDateTimeNow;

                CreateNewFile();

                stringBuilder.Append(Environment.NewLine);

                _writer.Write(stringBuilder.ToString());
                _writer.AutoFlush = true;
            }
        }

        private void CreateNewFile()
        {
            Thread.Sleep(400);
            _writer = File.AppendText(@"C:\LogTest\Log" + _systemClock.GetDateTimeNow.ToString("yyyyMMdd HHmmss fff") + ".log");
            _writer.Write("Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t" + Environment.NewLine);
        }
    }
}