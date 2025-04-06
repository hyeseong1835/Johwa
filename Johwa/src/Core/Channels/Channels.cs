using System.Text.Json;

namespace Johwa.Core.Channel;

public ref struct Channels
{
    public ref JsonElement JsonElement;
    public Channel Get(in JsonElement jsonElement, int index)
    {
        JsonElement channelJsonElement = jsonElement.GetProperty("channels")[index];

        return new Channel(channelJsonElement);
    }
    public int Count(in JsonElement jsonElement)
    {
        return jsonElement.GetProperty("channels").GetArrayLength();
    }
}