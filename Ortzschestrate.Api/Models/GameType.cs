using Ardalis.SmartEnum;

namespace Ortzschestrate.Api.Models;

public class GameType : SmartEnum<GameType>
{
    public static readonly GameType Untimed = new(nameof(Untimed), 0);
    public static readonly GameType Rapid = new(nameof(Rapid), 10);
    public static readonly GameType Blitz = new(nameof(Blitz), 5);
    public static readonly GameType Bullet = new(nameof(Bullet), 3);

    private GameType(string name, int value) : base(name, value)
    {
    }

    public TimeSpan GetTimeSpan()
    {
        if (Name == nameof(Untimed))
        {
            return TimeSpan.Zero;
        }
        
        return TimeSpan.FromMinutes(Value);
    }
}