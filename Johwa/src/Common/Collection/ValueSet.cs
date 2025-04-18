namespace Johwa.Common.Collection;

public readonly ref struct ValueSet<TValue, TIdValue>
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
    public ValueSet(ReadOnlyMemory<TValue> collection, Span<LinkedListNode> nodeBuffer)
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
    }
    
    public bool TryGetValue(TIdValue id, out TValue? value, Func<TValue, TIdValue, bool> idMatchFunc)
    {
        int index = list.headIndex;
        while (index != -1)
        {
            ref LinkedListNode node = ref list.nodes[index];
            value = list.values.Span[node.valueIndex];

            if (idMatchFunc.Invoke(value, id))
            {
                return true;
            }
            index = node.nextIndex;
        }
        value = default;
        return false;
    }
    public bool TryExtractValue(TIdValue id, out TValue? value, Func<TValue, TIdValue, bool> idMatchFunc)
    {
        int index = list.headIndex;
        if (index == -1) {
            value = default;
            return false;
        }

        ref LinkedListNode prevNode = ref list.HeadNode;
        ReadOnlySpan<TValue> valueSpan = list.values.Span;
        value = valueSpan[prevNode.valueIndex];
        
        if (idMatchFunc.Invoke(value, id)) {
            value = valueSpan[prevNode.valueIndex];
            list.SetHeadIndex(prevNode.nextIndex);
            prevNode.nextIndex = -1;
            return true;
        }
        index = prevNode.nextIndex;

        while (index != -1)
        {
            ref LinkedListNode node = ref list.nodes[index];
            value = valueSpan[node.valueIndex];
            if (idMatchFunc.Invoke(value, id))
            {
                prevNode.nextIndex = node.nextIndex;
                node.nextIndex = -1;

                return true;
            }
            index = node.nextIndex;
        }
        value = default;
        return false;
    }
    public void RemoveValueIf(TIdValue id, Func<TValue, TIdValue, bool> idMatchFunc)
    {
        int index = list.headIndex;
        if (index == -1) {
            return;
        }

        ref LinkedListNode prevNode = ref list.HeadNode;
        
        ReadOnlySpan<TValue> valueSpan = list.values.Span;
        TValue value = valueSpan[prevNode.valueIndex];
        
        if (idMatchFunc.Invoke(value, id)) 
        {
            list.SetHeadIndex(prevNode.nextIndex);
            prevNode.nextIndex = -1;
        }
        else 
        {
            prevNode = ref list.nodes[index];
            index = prevNode.nextIndex;
        }

        while (index != -1)
        {
            ref LinkedListNode node = ref list.nodes[index];
            value = valueSpan[node.valueIndex];

            if (idMatchFunc.Invoke(value, id))
            {
                prevNode.nextIndex = node.nextIndex;
                node.nextIndex = -1;
            }
            else 
            {
                prevNode = ref node;
                index = node.nextIndex;
            }
        }
    }
    public IEnumerable<TValue> ExtractValuesIf(TIdValue id, Func<TValue, TIdValue, bool> idMatchFunc)
    {
        int curNodeIndex = list.headIndex;
        if (curNodeIndex == -1) {
            yield break;
        }

        // 이전 노드를 시작 노드로 설정
        int prevNodeIndex = list.headIndex;
        LinkedListNode prevNode = list.HeadNode;
        
        // 시작 노드의 값을 가져옴
        ReadOnlySpan<TValue> valueSpan = list.values.Span;
        TValue value = valueSpan[prevNode.valueIndex];
        
        while (curNodeIndex != -1) 
        {
            // 시작 노드의 값과 id를 비교
            if (idMatchFunc.Invoke(value, id)) 
            {
                // 이전 노드 연결 해제
                list.nodes[prevNodeIndex].nextIndex = -1;

                // 시작 노드를 원래 이전 노드의 다음 노드로 설정
                list.SetHeadIndex(prevNode.nextIndex);
                
                yield return value;
            }
            else 
            {
                // 이전 노드 전진
                prevNode = list.nodes[prevNode.nextIndex];

                // 현재 노드 인덱스를 지금 이전 노드의 다음 노드 인덱스로 설정
                curNodeIndex = prevNode.nextIndex;
            }
        }
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