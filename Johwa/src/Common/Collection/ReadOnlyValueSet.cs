namespace Johwa.Common.Collection;

public readonly ref struct ReadOnlyValueSet<TValue, TIdValue>
    where TValue : ReadOnlyValueSet<TValue, TIdValue>.IIdentifier
{
    public interface IIdentifier
    {
        public bool IsMatch(TIdValue compareValue);
    }
    public struct LinkedListNode
    {
        public TValue value;
        public int nextIndex;

        public LinkedListNode(TValue value, int nextIndex)
        {
            this.value = value;
            this.nextIndex = nextIndex;
        }
    }
    ref struct LinkedList
    {
        public readonly Span<LinkedListNode> nodes;
        public int headIndex;
        public ref LinkedListNode HeadNode => ref nodes[headIndex];

        public LinkedList(Span<LinkedListNode> nodes, int headIndex)
        {
            this.nodes = nodes;
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

    ReadOnlyValueSet(Span<TValue> collection, Span<LinkedListNode> nodeBuffer)
    {
        for (int i = 0; i < collection.Length; i++)
        {
            ref TValue value = ref collection[i];

            if (i < collection.Length - 1)
            {
                nodeBuffer[i] = new LinkedListNode(value, i + 1);
            }
            else if (i == collection.Length - 1)
            {
                nodeBuffer[i] = new LinkedListNode(value, -1);
            }
            else 
            {
                throw new ArgumentOutOfRangeException(nameof(collection), "버퍼가 부족합니다.");
            }
        }
        list = new LinkedList(nodeBuffer, 0);
    }
    
    public bool TryGetValue(TIdValue id, out TValue? value)
    {
        int index = list.headIndex;
        while (index != -1)
        {
            ref LinkedListNode node = ref list.nodes[index];
            if (node.value.IsMatch(id))
            {
                value = node.value;
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
        if (prevNode.value.IsMatch(id)) {
            value = prevNode.value;
            list.SetHeadIndex(prevNode.nextIndex);
            return true;
        }
        index = prevNode.nextIndex;

        while (index != -1)
        {
            ref LinkedListNode node = ref list.nodes[index];
            if (node.value.IsMatch(id))
            {
                value = node.value;
                node.nextIndex = -1;
                
                prevNode.nextIndex = node.nextIndex;
                return true;
            }
            index = node.nextIndex;
        }
        value = default;
        return false;
    }
}