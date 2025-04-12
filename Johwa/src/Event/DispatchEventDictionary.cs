using System.Diagnostics.CodeAnalysis;

namespace Johwa.Event;

public class DispatchEventGroupDictionary
{
    Dictionary<int, DispatchEventGroup> dictionary;

    public DispatchEventGroupDictionary()
    {
        dictionary = new Dictionary<int, DispatchEventGroup>();
    }

    public void Add(string name, DispatchEventGroup dispatchEvent)
    {
        int hash = GetTextHash(name);
        dictionary.Add(hash, dispatchEvent);
    }
    public void Add(ReadOnlySpan<byte> name, DispatchEventGroup dispatchEvent)
    {
        int hash = GetTextHash(name);
        dictionary.Add(hash, dispatchEvent);
    }
    
    public DispatchEventGroup? GetValueOrNull(string name)
    {
        int hash = GetTextHash(name);
        
        if (dictionary.TryGetValue(hash, out DispatchEventGroup? dispatchEvent) == false)
            return null;

        return dispatchEvent;
    }
    public DispatchEventGroup? GetValueOrNull(ReadOnlySpan<byte> name)
    {
        int hash = GetTextHash(name);

        if (dictionary.TryGetValue(hash, out DispatchEventGroup? dispatchEvent) == false)
            return null;

        return dispatchEvent;
    }

    public bool TryGetValue(string name, [NotNullWhen(true)] out DispatchEventGroup? dispatchEvent)
    {
        int hash = GetTextHash(name);
        return dictionary.TryGetValue(hash, out dispatchEvent);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="dispatchEvent"></param>
    /// <returns></returns>
    public bool TryGetValue(ReadOnlySpan<byte> name, [NotNullWhen(true)] out DispatchEventGroup? dispatchEvent)
    {
        int hash = GetTextHash(name);
        return dictionary.TryGetValue(hash, out dispatchEvent);
    }
    
    int GetTextHash(string text)
    {
        int hash = 0;
        for (int i = 0; i < text.Length; i++)
        {
            hash = (hash * 31) + text[i];
        }
        return hash;
    }
    int GetTextHash(ReadOnlySpan<byte> span)
    {
        int hash = 0;
        for (int i = 0; i < span.Length; i++)
        {
            hash = (hash * 31) + span[i];
        }
        return hash;
    }
}