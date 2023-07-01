namespace LogComponent
{
    public interface ISystemClock
    {
        DateTimeOffset GetDateTimeNow { get; }
    }
}