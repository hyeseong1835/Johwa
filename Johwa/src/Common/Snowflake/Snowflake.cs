using System.Text.Json.Serialization;

namespace Johwa.Common;

[JsonConverter(typeof(SnowflakeJsonConverter))]
public readonly struct Snowflake : IEquatable<Snowflake>, IComparable<Snowflake>
{
    const long DiscordEpoch = 1420070400000L; // 2015-01-01T00:00:00.000Z
    public ulong Value { get; }

    public Snowflake(ulong value)
    {
        Value = value;
    }

    public DateTimeOffset CreatedAt
        => DateTimeOffset.FromUnixTimeMilliseconds(TimeStamp);
    public long TimeStamp 
        => (long)(Value >> 22) + DiscordEpoch;

    public override string ToString() => Value.ToString();

    public bool Equals(Snowflake other) => Value == other.Value;
    public override bool Equals(object obj) => obj is Snowflake other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();

    public int CompareTo(Snowflake other) => Value.CompareTo(other.Value);

    public static implicit operator ulong(Snowflake snowflake) => snowflake.Value;
    public static implicit operator Snowflake(ulong value) => new Snowflake(value);

    public static bool operator ==(Snowflake left, Snowflake right) => left.Equals(right);
    public static bool operator !=(Snowflake left, Snowflake right) => !(left == right);
    public static bool operator <(Snowflake left, Snowflake right) => left.Value < right.Value;
    public static bool operator >(Snowflake left, Snowflake right) => left.Value > right.Value;
}