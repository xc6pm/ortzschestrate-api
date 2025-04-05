using Ardalis.SmartEnum;

namespace Ortzschestrate.Api.Models;

public class GameType : SmartEnum<GameType>
{
    public static readonly GameType Untimed = new(nameof(Untimed), 0);
    public static readonly GameType Rapid = new(nameof(Rapid), 10);
    public static readonly GameType Blitz = new(nameof(Blitz), 5);
    public static readonly GameType Bullet3 = new(nameof(Bullet3), 3);
    public static readonly GameType Bullet1 = new(nameof(Bullet1), 1);
    public static readonly GameType Bullet10000 = new(nameof(Bullet10000), 10000);

    private GameType(string name, int value) : base(name, value)
    {
    }

    public TimeSpan GetTimeSpan()
    {
        if (this == Untimed)
        {
            return TimeSpan.Zero;
        }

        if (this == Bullet10000)
        {
            return TimeSpan.FromSeconds((10));
        }

        return TimeSpan.FromMinutes(Value);
    }
}