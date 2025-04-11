namespace Johwa.Common.Collection;

public readonly ref struct ReadOnlyValueSet<TValue, TIdValue>
{
    public struct LinkedListNode
    {
        public int valueIndex;
        public int nextIndex;

        public LinkedListNode(int valueIndex, int nextIndex)
        {
            this.valueIndex = valueIndex;
            this.nextIndex = nextIndex;
        }
    }
    ref struct LinkedList
    {
        public readonly Span<LinkedListNode> nodes;
        public readonly ReadOnlyMemory<TValue> values;

        public int headIndex;
        public ref LinkedListNode HeadNode => ref nodes[headIndex];

        public LinkedList(Span<LinkedListNode> nodes, ReadOnlyMemory<TValue> values, int headIndex)
        {
            this.nodes = nodes;
            this.values = values;
            this.headIndex = headIndex;
        }
        public int SetHeadIndex(int headIndex)
        {
            int oldHeadIndex = this.headIndex;
            this.headIndex = headIndex;
            return oldHeadIndex;
        }
    }
    
    readonly LinkedList list;
    readonly Func<TValue, TIdValue, bool> isMatchFunc;

    public ReadOnlyValueSet(ReadOnlyMemory<TValue> collection, Span<LinkedListNode> nodeBuffer, 
        Func<TValue, TIdValue, bool> isMatchFunc)
    {
        for (int i = 0; i < collection.Length; i++)
        {
            if (i < collection.Length - 1)
            {
                nodeBuffer[i] = new LinkedListNode(i, i + 1);
            }
            else if (i == collection.Length - 1)
            {
                nodeBuffer[i] = new LinkedListNode(i, -1);
            }
            else 
            {
                throw new ArgumentOutOfRangeException("버퍼가 부족합니다.");
            }
        }
        list = new LinkedList(nodeBuffer, collection, 0);
        this.isMatchFunc = isMatchFunc;
    }
    
    public bool TryGetValue(TIdValue id, out TValue? value)
    {
        int index = list.headIndex;
        while (index != -1)
        {
            ref LinkedListNode node = ref list.nodes[index];
            value = list.values.Span[node.valueIndex];

            if (isMatchFunc.Invoke(value, id))
            {
                return true;
            }
            index = node.nextIndex;
        }
        value = default;
        return false;
    }
    public bool TryGetExtractValue(TIdValue id, out TValue? value)
    {
        int index = list.headIndex;
        if (index == -1) {
            value = default;
            return false;
        }

        ref LinkedListNode prevNode = ref list.HeadNode;
        ReadOnlySpan<TValue> valueSpan = list.values.Span;
        value = valueSpan[prevNode.valueIndex];
        
        if (isMatchFunc.Invoke(value, id)) {
            value = valueSpan[prevNode.valueIndex];
            list.SetHeadIndex(prevNode.nextIndex);
            return true;
        }
        index = prevNode.nextIndex;

        while (index != -1)
        {
            ref LinkedListNode node = ref list.nodes[index];
            value = valueSpan[node.valueIndex];
            if (isMatchFunc.Invoke(value, id))
            {
                node.nextIndex = -1;

                prevNode.nextIndex = node.nextIndex;
                return true;
            }
            index = node.nextIndex;
        }
        value = default;
        return false;
    }
    public IEnumerable<TValue> GetEnumerable()
    {
        int index = list.headIndex;
        ReadOnlySpan<TValue> valueSpan = list.values.Span;

        while (index != -1)
        {
            LinkedListNode node = list.nodes[index];
            yield return valueSpan[node.valueIndex];
            index = node.nextIndex;
        }
    }
}