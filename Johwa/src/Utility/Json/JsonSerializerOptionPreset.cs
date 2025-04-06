
using System.Text.Json;

namespace Johwa.Utility.Json;

public static class JsonSerializerOptionPreset
{
    /// <summary>
    /// WriteIndented = true
    /// </summary>
    public static JsonSerializerOptions prettyPrint = new JsonSerializerOptions
    {
        WriteIndented = true
    };
}