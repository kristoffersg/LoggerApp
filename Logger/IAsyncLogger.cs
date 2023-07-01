namespace LogComponent
{
    public interface IAsyncLogger
    {
        /// <summary>
        /// Stop the logging. If any outstadning logs theses will not be written to Log
        /// </summary>
        void StopWithoutFlush();

        /// <summary>
        /// Stop the logging. The call will not return until all all logs have been written to Log.
        /// </summary>
        void StopWithFlush();

        /// <summary>
        /// WriteLog a message to the Log.
        /// </summary>
        /// <param name="s">The s to written to the log</param>
        void WriteToLog(string message);

        /// <summary>
        /// MainLoop, the log in it's own task
        /// </summary>
        void MainLoop();
    }
}
