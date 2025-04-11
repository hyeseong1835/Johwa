namespace Johwa.Common;

public interface IHasKey<TKey>
{
    TKey Key { get; }
}