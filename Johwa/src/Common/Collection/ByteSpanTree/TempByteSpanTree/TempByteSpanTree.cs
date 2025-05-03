using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Johwa.Common.Collection;

/// <summary>
/// 내부는 참조 변수로 이루어져 있기 때문에 복사하여도 정보를 공유함
/// </summary>
public partial struct UnmanagedByteSpanTree<TValue>
    where TValue : unmanaged
{
    #region Object
    
    unsafe internal struct ByteNode : IDisposable
    {
        #region Static

        public static ByteNode* Create(byte keyByte)
        {
            ByteNode* newNodePtr = (ByteNode*)Marshal.AllocHGlobal(sizeof(ByteNode));
            *newNodePtr = new ByteNode(keyByte);

            return newNodePtr;  
        }

        #endregion


        #region Instance

        public readonly byte keyByte;
        UnmanagedLinkedList<ByteNode> childList;
        
        ByteNode* parentPtr;
        public ByteNode* ParentPtr
            => parentPtr;
        public ref ByteNode ParentRef 
            => ref *parentPtr;

        TValue value;
        public TValue Value {
            get => value;
            set => SetValue(value);
        }

        bool hasValue;
        public bool HasValue => hasValue;


        #region 생성자

        /// <summary>
        /// 값 없는 노드 생성
        /// </summary>
        /// <param name="keyByte"></param>
        public ByteNode(byte keyByte)
        {
            this.keyByte = keyByte;
            this.childList = new ByteNode.List();

            this.value = default;
            this.hasValue = false;
        }

        /// <summary>
        /// 값 있는 노드 생성
        /// </summary>
        /// <param name="keyByte"></param>
        public ByteNode(byte keyByte, TValue value)
        {
            this.keyByte = keyByte;
            this.childList = new ByteNode.List();

            this.value = value;
            this.hasValue = true;
        }
    
        #endregion


        #region Method

        public bool TryGetValue(out TValue value)
        {
            if (hasValue)
            {
                value = this.value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public void SetValue(TValue value)
        {
            this.value = value;
            hasValue = true;
        }

        public ref ByteNode GetOrCreateChildRef(byte keyByte)
        {
            UnmanagedLinkedList<ByteNode>.RefEnumerator childListRefEnumerator 
                = new (ref childList);

            scoped UnmanagedLinkedList<ByteNode>.RefEnumerator.EnumerationInfo curInfo;
            scoped UnmanagedLinkedList<ByteNode>.RefEnumerator.EnumerationInfo prevInfo = new (childListRefEnumerator);
            
            while (childListRefEnumerator.MoveNext())
            {
                curInfo = childListRefEnumerator.Info;

                // 아직 찾지 못함 : keyByte < cur
                if (childListRefEnumerator.CurrentValuePtr->keyByte > keyByte)
                {
                    prevInfo = curInfo;
                    continue;
                }

                // 찾음 : keyByte == cur
                if (childListRefEnumerator.CurrentValuePtr->keyByte == keyByte)
                {
                    // 이미 자식 노드가 존재함 -> 추가하지 않음
                    return ref (*curInfo.ValuePtr);
                }

                // keyByte보다 작거나 같은 자식이 존재하지 않음
                if (prevInfo.IsNull)
                {
                    // 가장 앞에 삽입
                    childList.AddFirst(new ByteNode(keyByte));
                    return ref (*childList.HeadValuePtr);
                }
                // 현재 노드가 keyByte보다 큼
                else
                {
                    // 이전 노드 다음에 삽입
                    ByteNode newByteNode = new ByteNode(keyByte);

                    // 삽입 및 참조 반환
                    return ref (*prevInfo.InsertNextAndReturnPtr(newByteNode));
                }
            }

            // keyByte보다 크거나 같은 자식이 존재하지 않음 => 마지막에 삽입
            return ref childList.AddLastAndReturnRef(new ByteNode(keyByte));
        }
        public void Dispose()
        {
            childList.Dispose();
        }

        #endregion

        #endregion
    }

    public ref struct Enumerator
    {
        NodeEnumerator nodeEnumerator;

        public Enumerator(UnmanagedByteSpanTree<TValue> tree)
        {
            this.nodeEnumerator = tree.GetNodeIterator();
        }
        
        public bool MoveNext()
        {
            while (nodeEnumerator.MoveNext())
            {
                ref ByteNode currentNode = ref nodeEnumerator.CurrentRef;

                // 찾은 노드가 값이 있음 -> true 반환
                if (currentNode.HasValue)
                {
                    valueNodeRef = currentNode;
                    return true;
                }
                {
                    valueNodeRef = currentNode;
                    return true;
                }
            }
            return false;
        }
        public bool MoveNext([NotNullWhen(true)] out TValue? current)
        {
            while (nodeEnumerator.MoveNext())
            {
                // 찾은 노드가 값이 있음 -> true 반환
                if (nodeEnumerator.CurrentRef.HasValue)
                {
                    valueNodeRef = nodeEnumerator.CurrentRef;
                    current = valueNodeRef.Value;
                    return true;
                }
            }
            current = default;
            return false;
        }

        public void Dispose()
        {
            nodeEnumerator.Dispose();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
    
    unsafe internal ref struct NodeEnumerator
    {
        UnmanagedByteSpanTree<TValue> tree;

        UnmanagedStack<nint> findNodePtrStack;

        public ByteNode.ListNode* currentListNodePtr;
        public ByteNode* CurrentPtr => &currentListNodePtr->byteNode;
        public ref ByteNode CurrentRef => ref currentListNodePtr->byteNode;

        bool isEnd;


        // 생성자
        public NodeEnumerator(UnmanagedByteSpanTree<TValue> tree)
        {
            this.tree = tree;
            this.findNodePtrStack = new UnmanagedStack<nint>();

            this.currentListNodePtr = null;

            this.isEnd = false;
        }


        #region Method

        public void Dispose()
        {
            findNodeStack.Dispose();
        }

        public bool MoveNext()
        {
            // 모두 탐색함 -> false 반환
            if (isEnd) 
                return false;

            // 초기 상태이면 -> rootNode/true 반환
            if (currentListNodePtr == null) {
                currentListNodePtr = tree.rootByteNodeList.headNodePtr;
                findNodeStack.Push(-1);
                return true;
            }

            // 현재 노드가 브랜치 노드임 -> 찾을 노드가 남았으면 다음 노드/true 반환
            if (CurrentPtr->childList.Count > 0)
            {
                ref int nodeFindIndex = ref findNodeStack.Peek();

                // 아직 찾을 노드가 남았음 -> 다음 노드/true 반환
                if (nodeFindIndex + 1 < CurrentPtr->childList.Count)
                {
                    // 인덱스 이동
                    nodeFindIndex++;

                    // 다음 노드로 이동하고 반환
                    CurrentPtr = CurrentPtr.childNodeArray[nodeFindIndex];
                    findNodeStack.Push(-1);
                    return true;
                }
            }

            // 찾을 하위 노드가 없음 -> 탐색 가능한 상위 브랜치로 이동
            while(true)
            {
                // 마지막 찾기 인덱스를 제거
                findNodeStack.Pop();

                // 찾기 인덱스 스택 소진 -> true/null/false 반환
                if (findNodeStack.IsEmpty)
                {
                    isEnd = true;
                    CurrentPtr = null;
                    return false;
                }

                // 올라감
                ByteNode[]? lastBranch;
                if (UpToLastBranch(out lastBranch) == false)
                {
                    // 더 이상 올라갈 노드가 남지 않음
                    isEnd = true;
                    CurrentPtr = null;
                    return false;
                }

                ref int findNodeIndex = ref findNodeStack.Peek();

                // 브랜치에 찾을 노드가 남아있음
                if (findNodeIndex + 1 < lastBranch.Length)
                {
                    // 인덱스 이동
                    findNodeIndex++;

                    // 다음 노드로 이동하고 반환
                    CurrentPtr = lastBranch[findNodeIndex];
                    findNodeStack.Push(-1);
                    return true;
                }

                // 없으면 계속 올라감
            }
        }
        public bool MoveNext([NotNullWhen(true)] out ByteNode? current)
        {
            if (MoveNext())
            {
                current = this.CurrentPtr!;
                return true;
            }
            else
            {
                current = null;
                return false;
            }
        }
        
        bool UpToLastBranch([NotNullWhen(true)] out ByteNode[]? lastBranch)
        {
            while(CurrentPtr != null)
            {
                CurrentPtr = CurrentPtr.ParentNode;
                
                if (CurrentPtr == null) {
                    lastBranch = null;
                    return false;
                }

                if (CurrentPtr.childNodeArray == null)
                    throw new Exception("잘못된 노드가 존재합니다."); 
                
                if (CurrentPtr.childNodeArray.Length > 1) {
                    lastBranch = CurrentPtr.childNodeArray;
                    return false;
                }
            }

            lastBranch = null;
            return false;
        }
        
        #endregion
    }
    
    #endregion

    #region Instance

    #region 필드

    ByteNode rootByteNode;
    public int nodeCount;
    public int valueCount;
    public int maxNodeDepth;
    public int maxBranchDepth;

    #endregion


    // 생성자
    public UnmanagedByteSpanTree()
    {
        rootByteNodeList = new();
    }


    #region 메서드

    unsafe public void Add(ReadOnlySpan<byte> key, TValue value)
    {
        int keyLength = key.Length;

        if (keyLength == 0)
            throw new ArgumentException("키의 길이는 0보다 길어야 합니다.");


        ref ByteNode findByteNodePtr = ref rootByteNode;
        
        for (int i = 0; i < keyLength; i++)
        {
            byte keyByte = key[i];

            findByteNodePtr = ref findByteNodePtr.GetOrCreateChildRef(keyByte);
        }

        findByteNodePtr.SetValue(value);
    }


    #region Public

    public TValue GetValue(ReadOnlySpan<byte> key)
    {
        ByteNode node = GetNode(key);

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
        if (TryGetNode(key, out ByteNode? node))
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
    
    public Enumerator GetValueIterator()
        => new ValueIterator(this);
    public TValue[] ToArray()
    {
        TValue[] valueArray = new TValue[valueCount];

        using (Enumerator valueIterator = GetValueIterator())
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

    public void Dispose()
    {
        // 노드 리스트를 Dispose
        rootByteNodeList.Dispose();
    }

    #endregion


    #region Internal

    internal NodeEnumerator GetNodeIterator()
        => new NodeIterator(rootNode, maxBranchDepth);

    internal ByteNode GetNode(ReadOnlySpan<byte> key)
    {
        if (TryGetNode(key, out ByteNode? node))
        {
            return node;
        }
        else
        {
            throw new KeyNotFoundException($"'{Encoding.ASCII.GetString(key)}' 키를 찾을 수 없습니다.");
        }
    }

    internal bool TryGetNode(ReadOnlySpan<byte> key, [NotNullWhen(true)] out ByteNode? node)
    {
        // 키의 길이가 0임 -> false 반환
        if (key.Length == 0) {
            node = null;
            return false;
        }

        ByteNode? findNode = rootNode;
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