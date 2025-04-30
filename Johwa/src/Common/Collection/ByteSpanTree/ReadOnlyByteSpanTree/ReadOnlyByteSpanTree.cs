using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Collections;

namespace Johwa.Common.Collection;

public partial class ReadOnlyByteSpanTree<TValue> : IEnumerable<TValue>
{
    #region Object

    #region Public

    public struct ValueIterator : IEnumerator<TValue>, IDisposable
    {
        NodeIterator nodeIterator;

        Node? valueNode;
        internal ValueInfo CurrentValueInfo { get { 
            if (valueNode == null)
                throw new Exception("노드가 없습니다.");

            if (valueNode.valueInfo == null)
                throw new Exception("노드에 값이 없습니다.");

            return valueNode.valueInfo;
        } }
        
        public TValue Current { get { 
            if (valueNode == null)
                throw new Exception("노드가 없습니다.");

            if (valueNode.valueInfo == null)
                throw new Exception("노드에 값이 없습니다.");

            return valueNode.valueInfo.value;
        } }
        object? IEnumerator.Current { get { 
            if (valueNode == null)
                throw new Exception("노드가 없습니다.");

            if (valueNode.valueInfo == null)
                throw new Exception("노드에 값이 없습니다.");

            return valueNode.valueInfo.value;
        } }

        public ValueIterator(ReadOnlyByteSpanTree<TValue> tree)
        {
            this.nodeIterator = tree.GetNodeIterator();
        }
        
        public bool MoveNext()
        {
            Node? currentNode;
            while (nodeIterator.MoveNext(out currentNode))
            {
                // 찾은 노드가 값이 있음 -> true 반환
                if (currentNode.valueInfo != null)
                {
                    valueNode = currentNode;
                    return true;
                }
            }
            return false;
        }
        public bool MoveNext([NotNullWhen(true)] out TValue? current)
        {
            Node? currentNode;
            while (nodeIterator.MoveNext(out currentNode))
            {
                // 찾은 노드가 값이 있음 -> true 반환
                if (currentNode.valueInfo != null)
                {
                    valueNode = currentNode;
                    current = currentNode.valueInfo.value!;
                    return true;
                }
            }
            current = default;
            return false;
        }

        public void Dispose()
        {
            nodeIterator.Dispose();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
    
    #endregion


    #region Internal

    internal class Node
    {
        public int nodeIndex;
        public int valueIndex;
        public readonly byte[] keySlice;

        public readonly ValueInfo? valueInfo;

        public readonly int branchDepth;

        Node? parentNode;
        public Node? ParentNode => parentNode;
        public readonly Node[]? childNodeArray;


        #region Constructor

        public Node(int nodeIndex, byte[] keySlice, ValueInfo? valueInfo, Node[]? childNodeArray, int branchDepth)
        {
            this.nodeIndex = nodeIndex;
            this.keySlice = keySlice;
            
            this.valueInfo = valueInfo;

            this.childNodeArray = childNodeArray;
            if (childNodeArray != null)
            {
                for (int i = 0; i < childNodeArray.Length; i++)
                {
                    Node childNode = childNodeArray[i];
                    childNode.parentNode = this;
                }
            }
            this.branchDepth = branchDepth;
        }

        #endregion

        /// <summary>
        /// 이진 탐색으로 자식 노드를 찾음
        /// </summary>
        /// <param name="keySlice">찾을 keySlice</param>
        /// <param name="childNode">찾은 노드</param>
        /// <returns>지정한 keyByte를 가진 노드의 존재 여부</returns>
        public bool TryFindChildNode(ReadOnlySpan<byte> findKeySlice, [NotNullWhen(true)] out Node? childNode)
        {
            // 자식 노드가 없음 -> null/false
            if (childNodeArray == null) {
                childNode = null;
                return false;
            }

            int findKeySliceLength = findKeySlice.Length;
            byte targetNodeKeySliceStart = findKeySlice[0];

            int minIndex = 0;
            int maxIndex = childNodeArray.Length - 1;

            while (minIndex <= maxIndex)
            {
                int midIndex = (minIndex + maxIndex) / 2;
                Node midNode = childNodeArray[midIndex];
                byte[] midNodeKeySlice = midNode.keySlice;
                byte midNodeKeySliceStart = midNode.keySlice[0];

                // 가운데 노드 == 찾을 노드
                if (midNodeKeySliceStart == targetNodeKeySliceStart)
                {
                    int midNodeKeySliceLength = midNodeKeySlice.Length;

                    // 키 조각의 길이가 다름 -> null/false
                    if (midNodeKeySliceLength != findKeySliceLength) {
                        childNode = null;
                        return false;
                    }

                    // 키 조각 검사
                    for (int i = 1; i < midNodeKeySliceLength; i++)
                    {
                        // 키 조각이 다름 -> null/false
                        if (midNodeKeySlice[i] != findKeySlice[i]) {
                            childNode = null;
                            return false;
                        }
                    }

                    // 키 조각이 모두 같음 -> midNode/true
                    childNode = midNode;
                    return true;
                }
                // 가운데 노드 < 찾을 노드
                else if (midNodeKeySliceStart < targetNodeKeySliceStart)
                {
                    minIndex = midIndex + 1;
                }
                // 찾을 노드 < 가운데 노드
                else
                {
                    maxIndex = midIndex - 1;
                }
            }

            childNode = null;
            return false;
        }
    }
    
    internal class ValueInfo
    {
        public readonly TValue value;
        public readonly int valueIndex;

        public ValueInfo(TValue value, int valueIndex)
        {
            this.value = value;
            this.valueIndex = valueIndex;
        }
    }

    internal struct NodeIterator : IDisposable
    {
        public Node? Current => current;

        Node? current;

        UnmanagedStack<int> nodeFindIndexTrace;


        Node rootNode;
        bool isEnd;


        // 생성자
        public NodeIterator(Node rootNode, int maxBranchDepth)
        {
            this.nodeFindIndexTrace = new UnmanagedStack<int>(maxBranchDepth);

            this.current = null;
            this.rootNode = rootNode;
            isEnd = false;
        }


        #region Method

        public void Dispose()
        {
            nodeFindIndexTrace.Dispose();
        }

        public bool MoveNext()
        {
            // 모두 탐색함 -> false 반환
            if (isEnd) 
                return false;

            // 초기 상태이면 -> rootNode/true 반환
            if (current == null) {
                current = rootNode;
                nodeFindIndexTrace.Push(-1);
                return true;
            }

            // 현재 노드가 브랜치 노드임 -> 찾을 노드가 남았으면 다음 노드/true 반환
            if (current.childNodeArray != null)
            {
                ref int nodeFindIndex = ref nodeFindIndexTrace.Peek();

                // 아직 찾을 노드가 남았음 -> 다음 노드/true 반환
                if (nodeFindIndex + 1 < current.childNodeArray.Length)
                {
                    // 인덱스 이동
                    nodeFindIndex++;

                    // 다음 노드로 이동하고 반환
                    current = current.childNodeArray[nodeFindIndex];
                    nodeFindIndexTrace.Push(-1);
                    return true;
                }
            }

            // 찾을 하위 노드가 없음 -> 탐색 가능한 상위 브랜치로 이동
            while(true)
            {
                // 마지막 찾기 인덱스를 제거
                nodeFindIndexTrace.Pop();

                // 찾기 인덱스 스택 소진 -> true/null/false 반환
                if (nodeFindIndexTrace.IsEmpty)
                {
                    isEnd = true;
                    current = null;
                    return false;
                }

                // 올라감
                Node[]? lastBranch;
                if (UpToLastBranch(out lastBranch) == false)
                {
                    // 더 이상 올라갈 노드가 남지 않음
                    isEnd = true;
                    current = null;
                    return false;
                }

                ref int findNodeIndex = ref nodeFindIndexTrace.Peek();

                // 브랜치에 찾을 노드가 남아있음
                if (findNodeIndex + 1 < lastBranch.Length)
                {
                    // 인덱스 이동
                    findNodeIndex++;

                    // 다음 노드로 이동하고 반환
                    current = lastBranch[findNodeIndex];
                    nodeFindIndexTrace.Push(-1);
                    return true;
                }

                // 없으면 계속 올라감
            }
        }
        public bool MoveNext([NotNullWhen(true)] out Node? current)
        {
            if (MoveNext())
            {
                current = this.current!;
                return true;
            }
            else
            {
                current = null;
                return false;
            }
        }
        
        bool UpToLastBranch([NotNullWhen(true)] out Node[]? lastBranch)
        {
            while(current != null)
            {
                current = current.ParentNode;
                
                if (current == null) {
                    lastBranch = null;
                    return false;
                }

                if (current.childNodeArray == null)
                    throw new Exception("잘못된 노드가 존재합니다."); 
                
                if (current.childNodeArray.Length > 1) {
                    lastBranch = current.childNodeArray;
                    return false;
                }
            }

            lastBranch = null;
            return false;
        }
        
        #endregion
    }
    
    #endregion
    
    #endregion


    #region Static

    #region 노드 생성

    /// <summary>
    /// 값만 가진 노드 생성
    /// </summary>
    /// <param name="value"></param>
    internal static Node CreateValueNode(int nodeIndex, byte[] keySlice, ValueInfo valueInfo, int branchDepth)
    {
        return new Node(
            nodeIndex,
            keySlice,
            valueInfo,
            null,
            branchDepth
        );
    }

    /// <summary>
    /// 둘 이상의 자식 노드만 가진 노드 생성
    /// </summary>
    /// <param name="keySlice"></param>
    /// <param name="childNodeArray"></param>
    internal static Node CreateBranchNode(int nodeIndex, byte[] keySlice, Node[] childNodeArray, int branchDepth)
    {
        return new Node(
            nodeIndex,
            keySlice,
            null,
            childNodeArray,
            branchDepth
        );
    }

    /// <summary>
    /// 값과 하나 이상의 자식 노드를 모두 가지는 노드 생성
    /// </summary>
    /// <param name="keyByte"></param>
    /// <param name="childNodeInfo"></param>
    /// <param name="value"></param>
    internal static Node CreateValueBranchNode(int nodeIndex, byte[] keySlice, ValueInfo valueInfo, Node[] childNodeArray, int branchDepth)
    {
        return new Node(
            nodeIndex,
            keySlice,
            valueInfo,
            childNodeArray,
            branchDepth
        );
    }

    #endregion

    #region 루트 노드 생성

    internal static Node CreateEmptyTreeRootNode()
    {
        Node emptyNode
            = ReadOnlyByteSpanTree<TValue>.CreateBranchNode(
                0,
                new byte[0],
                new Node[0],
                0
            );

        return emptyNode;
    }

    internal static Node CreateValueTreeRootNode(byte[] keySlice, TValue value)
    {
        Node valueNode 
            = ReadOnlyByteSpanTree<TValue>.CreateValueNode(
                0, 
                keySlice, 
                new ValueInfo(value, 0),
                0
            );

        return valueNode;
    }

    internal static Node CreateBranchTreeRootNode(byte[] keySlice, Node[] childNodeArray)
    {
        Node branchNode
            = ReadOnlyByteSpanTree<TValue>.CreateBranchNode(
                0,
                keySlice,
                childNodeArray,
                1
            );

        return branchNode;
    }

    internal static Node CreateValueBranchTreeRootNode(byte[] keySlice, TValue value, Node[] childNodeArray)
    {
        Node valueBranchNode =
            ReadOnlyByteSpanTree<TValue>.CreateValueBranchNode(
                0, 
                keySlice,
                new ValueInfo(value, 0),
                childNodeArray,
                1
            );

        return valueBranchNode;
    }

    #endregion

    #endregion


    #region Instance
    
    readonly Node rootNode;
    public readonly int valueCount;
    public readonly int maxNodeDepth;
    public readonly int maxBranchDepth;

    internal ReadOnlyByteSpanTree(Node rootNode, int valueCount, int maxNodeDepth, int maxBranchDepth)
    {
        this.rootNode = rootNode;
        this.valueCount = valueCount;
        this.maxNodeDepth = maxNodeDepth;
        this.maxBranchDepth = maxBranchDepth;
    }


    #region Method

    #region 명시적 인터페이스 구현

    IEnumerator IEnumerable.GetEnumerator()
        => new ValueIterator(this);

    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        => new ValueIterator(this);

    #endregion


    #region Public

    public TValue GetValue(ReadOnlySpan<byte> key)
    {
        Node node = GetNode(key);

        if (node.valueInfo == null)
        {
            throw new KeyNotFoundException($"값이 존재하지 않습니다.");
        }
        else
        {
            return node.valueInfo.value;
        }
    }
    public bool TryGetValue(ReadOnlySpan<byte> key, [NotNullWhen(true)] out TValue? value)
    {
        if (TryGetNode(key, out Node? node))
        {
            if (node.valueInfo == null)
            {
                value = default;
                return false;
            }
            else
            {
                value = node.valueInfo.value!;
                return true;
            }
        }
        else
        {
            value = default;
            return false;
        }
    }
    
    public ValueIterator GetValueIterator()
        => new ValueIterator(this);
    public TValue[] ToArray()
    {
        TValue[] valueArray = new TValue[valueCount];

        using (ValueIterator valueIterator = GetValueIterator())
        {
            for (int i = 0; i < valueCount; i++)
            {
                TValue? currentValue;
                if (valueIterator.MoveNext(out currentValue))
                {
                    valueArray[i] = currentValue;
                }
                else
                {
                    throw new Exception("트리가 잘못되었습니다.");
                }
            }
        }

        return valueArray;
    }

    #endregion


    #region Internal

    internal NodeIterator GetNodeIterator()
        => new NodeIterator(rootNode, maxBranchDepth);

    internal Node GetNode(ReadOnlySpan<byte> key)
    {
        if (TryGetNode(key, out Node? node))
        {
            return node;
        }
        else
        {
            throw new KeyNotFoundException($"'{Encoding.ASCII.GetString(key)}' 키를 찾을 수 없습니다.");
        }
    }

    internal bool TryGetNode(ReadOnlySpan<byte> key, [NotNullWhen(true)] out Node? node)
    {
        // 키의 길이가 0임 -> false 반환
        if (key.Length == 0) {
            node = null;
            return false;
        }

        Node? findNode = rootNode;
        int keySliceStartIndex = 0;
           
        while (keySliceStartIndex < key.Length)
        {
            int findNodeKeySliceLength = findNode.keySlice.Length;

            // 자식 찾기
            if (findNode.TryFindChildNode(key.Slice(keySliceStartIndex, findNodeKeySliceLength), out findNode))
            {
                // 키 조각 시작 인덱스 업데이트
                keySliceStartIndex += findNodeKeySliceLength;
                continue;
            }
            else
            {
                // 자식을 찾을 수 없음
                node = null;
                return false;
            }
        }

        node = null;
        return false;
    }

    #endregion

    #endregion

    #endregion
}