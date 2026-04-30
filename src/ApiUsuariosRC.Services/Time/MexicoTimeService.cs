namespace ApiUsuariosRC.Services.Time;

public class MexicoTimeService : IMexicoTimeService
{
    private static readonly string[] TimeZoneIds =
    [
        "America/Mexico_City",
        "Central Standard Time (Mexico)",
        "Mexico Standard Time"
    ];

    private readonly TimeZoneInfo _timeZone;

    public MexicoTimeService()
    {
        _timeZone = ResolveTimeZone();
    }

    public DateTime GetCurrentMexicoCityDateTime()
    {
        return GetCurrentMexicoCityDateTimeOffset().DateTime;
    }

    public DateTimeOffset GetCurrentMexicoCityDateTimeOffset()
    {
        return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _timeZone);
    }

    private static TimeZoneInfo ResolveTimeZone()
    {
        foreach (var timeZoneId in TimeZoneIds)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        throw new InvalidOperationException("No fue posible resolver la zona horaria de Ciudad de Mexico.");
    }
}
