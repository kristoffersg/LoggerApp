namespace LogComponent
{
    public interface IAsyncLogger
    {
        void StopWithoutFlush();
        void StopWithFlush();
        void WriteToLog(string message);
        void MainLoop();
    }
}
