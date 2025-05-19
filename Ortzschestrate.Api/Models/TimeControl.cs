using Ardalis.SmartEnum;

namespace Ortzschestrate.Api.Models;

public class TimeControl : SmartEnum<TimeControl, double>
{
    private const double UntimedMilliseconds = -1;
    private const double RapidMilliseconds = 600000;
    private const double BlitzMilliseconds = 300000;
    private const double Bullet3Milliseconds = 180000;
    private const double Bullet1Milliseconds = 60000;
    private const double OneHourMilliseconds = 3600000;

    private const string OneHourKey = "1-hour";

    public static readonly TimeControl Untimed = new(nameof(Untimed), UntimedMilliseconds);
    public static readonly TimeControl Rapid = new(nameof(Rapid), RapidMilliseconds);
    public static readonly TimeControl Blitz = new(nameof(Blitz), BlitzMilliseconds);
    public static readonly TimeControl Bullet3 = new(nameof(Bullet3), Bullet3Milliseconds);
    public static readonly TimeControl Bullet1 = new(nameof(Bullet1), Bullet1Milliseconds);
    public static readonly TimeControl OneHour = new(OneHourKey, OneHourMilliseconds);

    private TimeControl(string name, double value) : base(name, value)
    {
    }

    public static TimeControl FromMilliseconds(double milliseconds) => milliseconds switch
    {
        UntimedMilliseconds => Untimed,
        RapidMilliseconds => Rapid,
        BlitzMilliseconds => Blitz,
        Bullet3Milliseconds => Bullet3,
        Bullet1Milliseconds => Bullet1,
        OneHourMilliseconds => OneHour,
        _ => throw new ArgumentOutOfRangeException($"Game type with {milliseconds} milliseconds is not defined.",
            nameof(milliseconds))
    };

    public static double ToMilliseconds(string name) =>
        name switch
        {
            nameof(Untimed) => UntimedMilliseconds,
            nameof(Rapid) => RapidMilliseconds,
            nameof(Blitz) => BlitzMilliseconds,
            nameof(Bullet3) => Bullet3Milliseconds,
            nameof(Bullet1) => Bullet1Milliseconds,
            OneHourKey => OneHourMilliseconds,
            _ => throw new ArgumentOutOfRangeException($"Game type named {name} is not defined.", nameof(name))
        };

    public TimeSpan GetTimeSpan()
    {
        if (this == Untimed)
        {
            return TimeSpan.Zero;
        }

        return TimeSpan.FromMilliseconds(Value);
    }
}