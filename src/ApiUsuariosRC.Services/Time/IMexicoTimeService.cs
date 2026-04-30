namespace ApiUsuariosRC.Services.Time;

public interface IMexicoTimeService
{
    DateTime GetCurrentMexicoCityDateTime();

    DateTimeOffset GetCurrentMexicoCityDateTimeOffset();
}
