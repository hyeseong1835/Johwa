using System.Text.Json;

namespace Johwa.Common.Json.Reader;

public ref struct DateTimeReader
{
    readonly ReadOnlySpan<byte> buffer;
    public DateTime DateTime { get {
        if (isReaded == false)
        {
            isReaded = true;
            return Get();
        }
        
        return _dateTime;
    } }
    public bool isReaded = false;
    DateTime _dateTime;

    public DateTimeReader(ReadOnlySpan<byte> buffer)
    {
        this.buffer = buffer;
    }

    public DateTime Get()
    {
        Utf8JsonReader reader = new Utf8JsonReader(buffer);
        return reader.GetDateTime();
    }

    public static implicit operator DateTime(DateTimeReader reader)
    {
        return reader.Get();
    }
}