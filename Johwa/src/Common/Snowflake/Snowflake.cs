using System.Buffers.Text;

namespace Johwa.Common;

public readonly struct Snowflake
{
    const long DiscordEpoch = 1420070400000L; // 2015-01-01T00:00:00.000Z
    public readonly ulong value;

    public Snowflake(ulong value)
    {
        this.value = value;
    }
    public static Snowflake Parse(ReadOnlySpan<byte> data)
    {
        if (data.Length <= 2) throw new ArgumentException("Invalid snowflake data length.");

        if (Utf8Parser.TryParse(data.Slice(1, data.Length - 2), out ulong result, out int consumed))
        {
            if (consumed != data.Length) throw new ArgumentException("Invalid snowflake data.");

            return new Snowflake(result);
        }
        else
        {
            throw new ArgumentException("Invalid snowflake data.");
        }
    }

    public DateTimeOffset CreatedAt
        => DateTimeOffset.FromUnixTimeMilliseconds(TimeStamp);
    public long TimeStamp 
        => (long)(value >> 22) + DiscordEpoch;

    public override string ToString() => value.ToString();

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }
    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public static implicit operator ulong(Snowflake snowflake) => snowflake.value;
    public static implicit operator Snowflake(ulong value) => new Snowflake(value);

    public static bool operator ==(Snowflake left, Snowflake right) => left.value == right.value;
    public static bool operator !=(Snowflake left, Snowflake right) => left.value != right.value;
}