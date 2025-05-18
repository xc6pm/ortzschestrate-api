using Ardalis.SmartEnum;

namespace Ortzschestrate.Api.Models;

public class TimeControl : SmartEnum<TimeControl>
{
    private const int UntimedMilliseconds = -1;
    private const int RapidMilliseconds = 600000;
    private const int BlitzMilliseconds = 300000;
    private const int Bullet3Milliseconds = 180000;
    private const int Bullet1Milliseconds = 60000;

    public static readonly TimeControl Untimed = new(nameof(Untimed), UntimedMilliseconds);
    public static readonly TimeControl Rapid = new(nameof(Rapid), RapidMilliseconds);
    public static readonly TimeControl Blitz = new(nameof(Blitz), BlitzMilliseconds);
    public static readonly TimeControl Bullet3 = new(nameof(Bullet3), Bullet3Milliseconds);
    public static readonly TimeControl Bullet1 = new(nameof(Bullet1), Bullet1Milliseconds);

    private TimeControl(string name, int value) : base(name, value)
    {
    }

    public static TimeControl FromMilliseconds(int milliseconds) => milliseconds switch
    {
        UntimedMilliseconds => Untimed,
        RapidMilliseconds => Rapid,
        BlitzMilliseconds => Blitz,
        Bullet3Milliseconds => Bullet3,
        Bullet1Milliseconds => Bullet1,
        _ => throw new ArgumentOutOfRangeException($"Game type with {milliseconds} milliseconds is not defined.",
            nameof(milliseconds))
    };

    public TimeSpan GetTimeSpan()
    {
        if (this == Untimed)
        {
            return TimeSpan.Zero;
        }

        return TimeSpan.FromMinutes(Value);
    }
}