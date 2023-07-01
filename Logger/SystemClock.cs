namespace LogComponent
{
    public class SystemClock : ISystemClock
    {
        public DateTimeOffset GetDateTimeNow => DateTimeOffset.Now;
    }
}
