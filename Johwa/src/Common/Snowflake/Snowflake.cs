using System.Buffers.Text;

namespace Johwa.Common;

/// <summary>
/// 63 ~ 22 (42 bits) : Timestamp             2015-01-01T00:00:00.000Z 이후 지난 시간(ms) Milliseconds since Discord Epoch, the first second of 2015 or 1420070400000.
/// 21 ~ 17 (5 bits)  : Internal worker ID    
/// 16 ~ 12 (5 bits)  : Internal process ID   
/// 11 ~ 0  (12 bits) : Increment		      For every ID that is generated on that process, this number is incremented
/// </summary>
public readonly struct Snowflake
{
    const long DiscordEpoch = 1420070400000L; // 2015-01-01T00:00:00.000Z
    public readonly ulong value;

    public long TimeStamp 
        => (long)(value >> 22) + DiscordEpoch;
    
    public ulong InternalWorkerID
        => (value & 0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0011_1110_0000_0000_0000_0000) >> 17;

    public ulong InternalProcessID
        => (value & 0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0001_1111_0000_0000_0000) >> 12;
    
    public ulong Increment
        => value & 0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111_1111_1111;

    public Snowflake(ulong value)
    {
        this.value = value;
    }
    public static Snowflake Parse(ReadOnlySpan<byte> data)
    {
        if (data.Length <= 2) throw new ArgumentException("Invalid snowflake data length.");

        ReadOnlySpan<byte> valueString = data.Slice(1, data.Length - 2);

        ulong result;
        int consumed;
        if (Utf8Parser.TryParse(valueString, out result, out consumed) == false)
            throw new ArgumentException("Invalid snowflake data.");

        if (consumed != data.Length) throw new ArgumentException("Invalid snowflake data.");

        return new Snowflake(result);
    }

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