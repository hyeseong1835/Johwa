#pragma warning disable CS8500 // 주소를 가져오거나, 크기를 가져오거나, 관리되는 형식에 대한 포인터를 선언합니다.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace Johwa.Common.Collection;

public class ReadOnlyByteSpanTree<TValue>
{
    #region Objects

    public ref struct Builder
    {
        #region Object

        unsafe public ref struct ByteNodeList
        {
            #region Object

            unsafe ref struct Node
            {
                public ByteNode byteNode;
                public Node* next;

                public Node(ByteNode byteNode, Node* next)
                {
                    this.next = next;
                    this.byteNode = byteNode;
                }
            }
            
            unsafe public ref struct Iterator
            {
                Node* nodePtr;

                public ref ByteNode Current { get {
                    if (nodePtr == null) { 
                        throw new InvalidOperationException("현재 노드가 없습니다.");
                    }

                    return ref nodePtr->byteNode;
                } }

                public Iterator(ByteNodeList byteNodeList)
                {
                    this.nodePtr = byteNodeList.headNodePtr;
                }

                public bool MoveNext()
                {
                    if (nodePtr == null) return false;

                    nodePtr = nodePtr->next;

                    return nodePtr != null;
                }
            }
            
            #endregion


            #region Instance

            // Field & Property
            Node* headNodePtr;
            int count;

            public ref ByteNode HeadByteNode => ref headNodePtr->byteNode;
            public int Count => count;
            public bool IsEmpty => headNodePtr == null;


            // Constructor
            public ByteNodeList()
            {
                headNodePtr = null;
                count = 0;
            }


            #region Method

            public ref ByteNode GetOrCreateByteNode(byte keyByte)
            {
                // 헤드 노드가 없음 -> 헤드 노드를 생성 후 반환
                if (headNodePtr == null)
                {
                    headNodePtr = (Node*)Marshal.AllocHGlobal(sizeof(Node));

                    // 생성 : head(new)
                    *headNodePtr = new Node(
                        new ByteNode(keyByte),
                        null
                    );

                    // 개수 업데이트
                    count = 1;

                    // 반환
                    return ref headNodePtr->byteNode;
                }

                Node* findNodePtr = headNodePtr;
                Node* prevNodePtr = null;

                while (findNodePtr != null)
                {
                    byte findKeyByte = findNodePtr->byteNode.keyByte;

                    // 일치하는 키를 찾음 -> findNode의 바이트 노드를 반환
                    if (findKeyByte == keyByte)
                    {
                        return ref findNodePtr->byteNode;
                    }
                    
                    // 아직 찾지 못함 -> 다음 노드로 이동
                    if (findKeyByte < keyByte)
                    {
                        prevNodePtr = findNodePtr;
                        findNodePtr = findNodePtr->next;
                        continue;
                    }

                    // 키를 찾지 못함

                    // 새로운 노드를 생성
                    Node* newNodePtr = (Node*)Marshal.AllocHGlobal(sizeof(Node));

                    // prev -> new
                    if (prevNodePtr != null)
                    {
                        prevNodePtr->next = newNodePtr;
                    }

                    // new -> find
                    *newNodePtr = new Node(
                        new ByteNode(keyByte),
                        findNodePtr
                    );

                    // 개수 업데이트
                    count++;

                    // 반환
                    return ref newNodePtr->byteNode;
                }

                // 끝까지 도달 -> 마지막 노드 뒤에 추가

                prevNodePtr->next = (Node*)Marshal.AllocHGlobal(sizeof(Node));
                
                // prev -> new
                *prevNodePtr->next = new Node(
                    new ByteNode(keyByte), 
                    null
                );

                // 개수 업데이트
                count++;

                // 반환
                return ref prevNodePtr->byteNode;
            }
            public void Dispose()
            {
                Node* findNodePtr = headNodePtr;
                Node* nextNodePtr = null;

                while (findNodePtr != null)
                {
                    // 찾은 노드의 자식 리스트 Dispose
                    findNodePtr->byteNode.childList.Dispose();

                    // 임시 변수
                    nextNodePtr = findNodePtr->next;
                    
                    // 찾은 노드 해제
                    Marshal.FreeHGlobal((IntPtr)findNodePtr);

                    // 다음 노드로 이동
                    findNodePtr = nextNodePtr;
                }
            }
            public Iterator GetIterator()
               => new Iterator(this);
            
            #endregion

            #endregion
        }
        
        public ref struct ByteNode
        {
            public readonly byte keyByte;
            public readonly ByteNodeList childList;

            TValue? value;
            bool hasValue;
            public bool HasValue => hasValue;


            #region 생성자

            /// <summary>
            /// 노드 생성
            /// </summary>
            /// <param name="keyByte"></param>
            public ByteNode(byte keyByte)
            {
                this.keyByte = keyByte;
                this.childList = new ByteNodeList();

                this.value = default;
                this.hasValue = false;
            }
        
            #endregion


            public bool TryGetValue([NotNullWhen(true)] out TValue? value)
            {
                if (hasValue)
                {
                    value = this.value!;
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }

            internal void SetValue(TValue value)
            {
                this.value = value;
                hasValue = true;
            }
        }
        
        struct BuildData
        {
            public int nodeCount;
            public int valueCount;
            public int maxNodeDepth;

            public BuildData()
            {
                nodeCount = 0;
                valueCount = 0;
                maxNodeDepth = 0;
            }
        }
        
        #endregion


        #region Static

        #region 트리 생성        

        static ReadOnlyByteSpanTree<TValue> CreateEmptyTree()
        {
            ChildNodeInfo rootChildNodeInfo 
                = new (
                    [ ], 
                    default
                );

            return new ReadOnlyByteSpanTree<TValue>(rootChildNodeInfo, 0, 1);
        }
        static ReadOnlyByteSpanTree<TValue> CreateㅍTree()
        {
            ChildNodeInfo rootChildNodeInfo 
                = new (
                    [ ], 
                    default
                );

            return new ReadOnlyByteSpanTree<TValue>(rootChildNodeInfo, 0, 1);
        }
        static ReadOnlyByteSpanTree<TValue> CreateBranchRootTree(in ByteNodeList byteNodeList, int byteNodeListDepth)
        {
            BuildData buildData = new ();
            Node[] childNodeArray = CreateNodeArray(
                -1,
                byteNodeList, 
                byteNodeListDepth,
                ref buildData
            );

            ChildNodeInfo rootChildNodeInfo = new ChildNodeInfo(
                childNodeArray,
                byteNodeListDepth
            );

            return new ReadOnlyByteSpanTree<TValue>(rootChildNodeInfo, buildData.valueCount, buildData.maxNodeDepth);
        }
        static ReadOnlyByteSpanTree<TValue> CreateValueBranchTree(in ByteNode byteNode, in TValue value, in ByteNodeList byteNodeList, int byteNodeListDepth)
        {
            BuildData buildData = new();

            Node[] childNodeArray = CreateNodeArray(
                0,
                byteNodeList, 
                byteNodeListDepth,
                ref buildData
            );
            
            ChildNodeInfo childNodeInfo = new ChildNodeInfo(
                childNodeArray, 
                byteNodeListDepth
            );
            Node node = ReadOnlyByteSpanTree<TValue>.CreateValueBranchNode(
                0,
                byteNode.keyByte,
                value,
                childNodeInfo
            );
            ChildNodeInfo rootChildNodeInfo = new ChildNodeInfo(
                [ node ],
                byteNodeListDepth
            );

            return new ReadOnlyByteSpanTree<TValue>(rootChildNodeInfo, buildData.valueCount, buildData.maxNodeDepth);
        }

        #endregion

        #region 노드 생성

        /// <summary>
        /// 값 노드 생성
        /// </summary>
        /// <param name="value"></param>
        static Node CreateValueNode(int nodeIndex, byte keyByte, TValue value, int nodeDepth, ref BuildData buildData)
        {
            buildData.maxNodeDepth = Math.Max(buildData.maxNodeDepth, nodeDepth);
            buildData.nodeCount++;

            buildData.valueCount++;

            return ReadOnlyByteSpanTree<TValue>.CreateValueNode(nodeIndex, keyByte, value);
        }

        /// <summary>
        /// 브랜치 노드 생성
        /// </summary>
        /// <param name="keyByte"></param>
        /// <param name="childNodeInfo"></param>
        static Node CreateBranchNode(int nodeIndex, byte keyByte, ChildNodeInfo childNodeInfo, ref BuildData buildData)
        {
            buildData.nodeCount++;

            return ReadOnlyByteSpanTree<TValue>.CreateBranchNode(nodeIndex, keyByte, childNodeInfo);
        }

        /// <summary>
        /// 값 브랜치 노드 생성
        /// </summary>
        /// <param name="keyByte"></param>
        /// <param name="childNodeInfo"></param>
        /// <param name="value"></param>
        static Node CreateValueBranchNode(int nodeIndex, byte keyByte, TValue value, ChildNodeInfo childNodeInfo, ref BuildData buildData)
        {
            buildData.nodeCount++;

            buildData.valueCount++;

            return ReadOnlyByteSpanTree<TValue>.CreateValueBranchNode(nodeIndex, keyByte, value, childNodeInfo);
        }

        #endregion

        static Node CreateNode(ref ByteNode byteNode, int nodeIndex, int nodeDepth, int byteIndex, ref BuildData buildData)
        {
            int moveCount;
            ref ByteNode nextNode = ref GetNextValueOrBranchNode(ref byteNode, out moveCount);

            if (nextNode.childList.IsEmpty)
            {
                TValue? value;
                if (nextNode.TryGetValue(out value))
                {
                    return CreateValueNode(
                        nodeIndex,
                        nextNode.keyByte,
                        value,
                        nodeDepth,
                        ref buildData
                    );
                }
                else
                {
                    throw new InvalidOperationException("잘못된 노드가 존재합니다.");
                }
            }
            else
            {
                Node[] childNodeArray = CreateNodeArray(
                    nodeIndex,
                    nextNode.childList,
                    byteIndex + 1,
                    ref buildData
                );

                ChildNodeInfo childNodeInfo = new ChildNodeInfo(
                    childNodeArray,
                    byteIndex + moveCount
                );
                
                TValue? value;
                if (nextNode.TryGetValue(out value))
                {
                    return CreateValueBranchNode(
                        nodeIndex,
                        nextNode.keyByte,
                        value,
                        childNodeInfo,
                        ref buildData
                    );
                }
                else
                {
                    return CreateBranchNode(
                        nodeIndex,
                        nextNode.keyByte,
                        childNodeInfo,
                        ref buildData
                    );
                }
            }
        }
        
        static Node[] CreateNodeArray(int parentNodeIndex, ByteNodeList byteNodeList, int byteIndex, ref BuildData buildData)
        {
            Node[] childArray = new Node[byteNodeList.Count];

            ByteNodeList.Iterator byteNodeIterator = byteNodeList.GetIterator();

            maxNodeDepth = -1;
            for (int i = 0; i < byteNodeList.Count; i++)
            {
                byteNodeIterator.MoveNext();
                ref ByteNode byteNode = ref byteNodeIterator.Current;

                int valueCount;
                int childMaxNodeDepth;

                // 자식 리스트를 빌드
                childArray[i] = CreateNode(
                    parentNodeIndex + i + 1,
                    byteNode.childList, 
                    byteIndex,
                    out valueCount,
                    out childMaxNodeDepth
                );

                childValueCount += valueCount;
                maxNodeDepth = Math.Max(maxNodeDepth, childMaxNodeDepth);
            }

            return childArray;
        }

        static ref ByteNode GetNextValueOrBranchNode(ref ByteNode startNode, out int moveCount)
        {
            moveCount = 0;

            ref ByteNode findByteNode = ref startNode;

            while (true)
            {
                // 노드가 값을 가지고 있음 -> 반환
                if (findByteNode.HasValue)
                {
                    return ref findByteNode;
                }

                // 자식 노드가 1개임 -> 다음 depth로 이동
                if (findByteNode.childList.Count == 1)
                {
                    moveCount++;
                    findByteNode = findByteNode.childList.HeadByteNode;
                    continue;
                }

                // 자식 노드가 2개 이상 -> 반환
                if (findByteNode.childList.Count > 1)
                {
                    return ref findByteNode;
                }

                // 노드 리스트가 비어있음 -> 예외 발생
                throw new ArgumentException("노드 리스트가 비어 있습니다.");
            }
        }
        
        #endregion


        #region Instance

        #region 필드

        ByteNodeList nodeList;

        #endregion


        // 생성자
        public Builder()
        {
            nodeList = new();
        }


        #region 메서드

        public void Add(ReadOnlySpan<byte> key, TValue value)
        {
            if (key.Length == 0)
            {
                throw new ArgumentException("키의 길이는 0보다 길어야 합니다.");
            }

            byte keyByte = key[0];

            ref ByteNode findNode = ref nodeList.GetOrCreateByteNode(keyByte);

            int findByteIndex = 0;
            int keyLength = key.Length;

            while (findByteIndex < keyLength)
            {
                // 키 바이트
                keyByte = key[findByteIndex];

                // 다음으로 이동
                findNode = ref findNode.childList.GetOrCreateByteNode(keyByte);
                findByteIndex++;
            }

            // 찾은 노드에 값을 설정
            findNode.SetValue(value);
        }

        public ReadOnlyByteSpanTree<TValue> Build()
        {
            // 노드가 비었음 -> 빈 트리 반환
            if (nodeList.IsEmpty)
            {
                return CreateEmptyTree();
            }

            ref ByteNodeList findByteNodeList = ref nodeList;
            int findByteNodeListDepth = 0;

            while (true)
            {
                // 노드 리스트의 노드 꺼내기
                ref ByteNode byteNode = ref findByteNodeList.HeadByteNode;

                TValue? value;

                // 노드가 값을 가지고 있음 -> 값 루트 노드 생성
                if (byteNode.TryGetValue(out value))
                {
                    // 노드의 자식 노드 리스트가 비어있음 -> 값 루트 노드 생성
                    if (byteNode.childList.IsEmpty)
                    {
                        return CreateValueTree(in byteNode, in value, findByteNodeListDepth);
                    }
                    // 자식 노드 리스트가 비어있지 않음 -> 값 브랜치 루트 노드 생성
                    else
                    {
                        return CreateValueBranchTree(in byteNode, in value, in findByteNodeList, findByteNodeListDepth);
                    }
                }

                // 자식 노드가 1개임 -> 다음 depth로 이동
                if (findByteNodeList.Count == 1)
                {
                    findByteNodeList = findByteNodeList.HeadByteNode.childList;
                    findByteNodeListDepth++;
                    continue;
                }

                // 자식 노드가 2개 이상 -> 브랜치 루트 노드 생성
                if (findByteNodeList.Count > 1)
                {
                    return CreateBranchRootTree(in findByteNodeList, findByteNodeListDepth);
                }

                // 노드 리스트가 비어있음 -> 예외 발생
                if (findByteNodeList.IsEmpty)
                {
                    throw new ArgumentException("노드 리스트가 비어 있습니다.");
                }
            }
        }
        
        public void Dispose()
        {
            // 노드 리스트를 Dispose
            nodeList.Dispose();
        }

        public ReadOnlyByteSpanTree<TValue> BuildAndDispose()
        {
            // 트리 빌드
            ReadOnlyByteSpanTree<TValue> tree = Build();

            // Dispose
            Dispose();
            
            // 빌드한 트리 반환
            return tree;
        }

        #endregion

        #endregion
    }

    public struct ValueIterator : IDisposable
    {
        NodeIterator nodeIterator;

        TValue? current;

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
                if (currentNode.TryGetValue(out current))
                {
                    return true;
                }
            }
            return false;
        }
        public bool MoveNext([NotNullWhen(true)] out TValue? current)
        {
            if (MoveNext())
            {
                current = this.current!;
                return true;
            }
            else
            {
                current = default;
                return false;
            }
        }

        public void Dispose()
        {
            nodeIterator.Dispose();
        }
    }

    class Node
    {
        public int nodeIndex;
        public readonly byte keyByte;

        public readonly TValue? value;
        public readonly bool hasValue;

        public readonly ChildNodeInfo? childNodeInfo;


        #region Constructor

        public Node(int nodeIndex, byte keyByte, TValue? value, bool hasValue, ChildNodeInfo? childNodeInfo)
        {
            this.nodeIndex = nodeIndex;
            this.keyByte = keyByte;
            this.value = value;
            this.hasValue = hasValue;
            this.childNodeInfo = childNodeInfo;
        }

        #endregion

        public bool TryGetValue([NotNullWhen(true)] out TValue? value)
        {
            if (hasValue)
            {
                value = this.value!;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
    
    class ChildNodeInfo
    {
        public readonly Node[] childNodeArray;
        public readonly int findByteIndex;

        public ChildNodeInfo(Node[] childNodeArray, int findByteIndex)
        {
            this.childNodeArray = childNodeArray;
            this.findByteIndex = findByteIndex;
        }

        /// <summary>
        /// 이진 탐색으로 자식 노드를 찾음
        /// </summary>
        /// <param name="keyByte">찾을 keyByte</param>
        /// <param name="childNode">찾은 노드</param>
        /// <returns>지정한 keyByte를 가진 노드의 존재 여부</returns>
        public bool TryFindChildNode(byte keyByte, [NotNullWhen(true)] out Node? childNode)
        {
            int minIndex = 0;
            int maxIndex = childNodeArray.Length - 1;
            
            while (minIndex <= maxIndex)
            {
                int midIndex = (minIndex + maxIndex) / 2;
                Node midNode = childNodeArray[midIndex];

                // 가운데 노드 == 찾을 노드
                if (midNode.keyByte == keyByte)
                {
                    childNode = midNode;
                    return true;
                }
                // 가운데 노드 < 찾을 노드
                else if (midNode.keyByte < keyByte)
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

    struct NodeIterator : IDisposable
    {
        unsafe struct NodeFindInfo
        {
            public readonly ChildNodeInfo* childNodeInfo;
            public int childNodeFindIndex;

            public NodeFindInfo(ChildNodeInfo childNodeInfo, int childNodeFindIndex = -1)
            {
                this.childNodeInfo = &childNodeInfo;
                this.childNodeFindIndex = childNodeFindIndex;
            }
            public bool TryGetNode([NotNullWhen(true)] out Node? node)
            {
                Node[] childNodeArray = childNodeInfo->childNodeArray;

                if (childNodeFindIndex < childNodeArray.Length)
                {
                    node = childNodeArray[childNodeFindIndex];
                    return true;
                }
                else
                {
                    node = null;
                    return false;
                }
            }
            public bool TryMoveNext([NotNullWhen(true)] out Node? node)
            {
                childNodeFindIndex++;

                if (TryGetNode(out node))
                {
                    return true;
                }
                else
                {
                    childNodeFindIndex--;
                    return false;
                }
            }
        }
        Node? current;

        UnmanagedStack<NodeFindInfo> nodeFindInfoStack;
        GCHandle treeGCHandle;

        public Node? Current => current;

        public NodeIterator(ReadOnlyByteSpanTree<TValue> tree)
        {
            treeGCHandle = GCHandle.Alloc(tree, GCHandleType.Pinned);
            nodeFindInfoStack = new UnmanagedStack<NodeFindInfo>(tree.maxNodeDepth);
            nodeFindInfoStack.Push(new NodeFindInfo(tree.rootChildNodeInfo));
        }
        public bool MoveNext()
        {
            if (nodeFindInfoStack.IsEmpty)
            {
                current = null;
                return false;
            }

            NodeFindInfo nodeFindInfo;
            while (nodeFindInfoStack.TryPop(out nodeFindInfo))
            {
                Node? node;
                if (nodeFindInfo.TryMoveNext(out node))
                {
                    // 찾을 노드가 남아있음 -> true 반환
                    current = node;
                    return true;
                }
            }
            
            // 모든 노드 스택 소모 -> false 반환
            current = null;
            return false;
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
        public void Dispose()
        {
            nodeFindInfoStack.Dispose();
            treeGCHandle.Free();
        }
    }
    
    #endregion


    #region Static

    /// <summary>
    /// 값 노드 생성
    /// </summary>
    /// <param name="value"></param>
    static Node CreateValueNode(int nodeIndex, byte keyByte, TValue value)
    {
        return new Node(
            nodeIndex,
            keyByte,
            value,
            true,
            null
        );
    }

    /// <summary>
    /// 브랜치 노드 생성
    /// </summary>
    /// <param name="keyByte"></param>
    /// <param name="childNodeInfo"></param>
    static Node CreateBranchNode(int nodeIndex, byte keyByte, ChildNodeInfo childNodeInfo)
    {
        return new Node(
            nodeIndex,
            keyByte,
            default,
            false,
            childNodeInfo
        );
    }

    /// <summary>
    /// 값 브랜치 노드 생성
    /// </summary>
    /// <param name="keyByte"></param>
    /// <param name="childNodeInfo"></param>
    /// <param name="value"></param>
    static Node CreateValueBranchNode(int nodeIndex, byte keyByte,  TValue value, ChildNodeInfo childNodeInfo)
    {
        return new Node(
            nodeIndex,
            keyByte,
            value,
            true,
            childNodeInfo
        );
    }

    static ReadOnlyByteSpanTree<TNewValue>.Node CreateNewNode<TNewValue>(Node originalNode, Func<TValue, TNewValue> valueSelector)
    {
        TValue? originalValue;

        //값이 있음
        if (originalNode.TryGetValue(out originalValue))
        {
            if (originalNode.childNodeInfo == null)
            {
                // 값 노드 생성
                return ReadOnlyByteSpanTree<TNewValue>.Builder.CreateValueNode(
                    originalNode.keyByte,
                    valueSelector.Invoke(originalValue)
                );
            }
            else
            {
                ReadOnlyByteSpanTree<TNewValue>.ChildNodeInfo newChildNodeInfo 
                    = CreateNewChildNodeInfo(originalNode.childNodeInfo, valueSelector);

                // 값 브랜치 노드 생성
                return ReadOnlyByteSpanTree<TNewValue>.CreateValueBranchNode(
                    originalNode.keyByte,
                    valueSelector.Invoke(originalValue),
                    newChildNodeInfo
                );
            }
        }
        else
        {
            if (originalNode.childNodeInfo == null)
            {
                throw new ArgumentException("잘못된 노드가 존재합니다.");
            }
            else
            {
                // 브랜치 노드 생성
                ReadOnlyByteSpanTree<TNewValue>.ChildNodeInfo newChildNodeInfo 
                    = CreateNewChildNodeInfo(originalNode.childNodeInfo, valueSelector);

                return ReadOnlyByteSpanTree<TNewValue>.CreateBranchNode(
                    originalNode.keyByte,
                    newChildNodeInfo
                );
            }
        }
    }
    
    static ReadOnlyByteSpanTree<TNewValue>.ChildNodeInfo CreateNewChildNodeInfo<TNewValue>(ChildNodeInfo originalChildInfo, Func<TValue, TNewValue> valueSelector)
    {
        ReadOnlyByteSpanTree<TNewValue>.Node[] newChildNodeArray 
            = new ReadOnlyByteSpanTree<TNewValue>.Node[originalChildInfo.childNodeArray.Length];

        for (int i = 0; i < newChildNodeArray.Length; i++)
        {
            Node originalChildNode = originalChildInfo.childNodeArray[i];
            newChildNodeArray[i] = CreateNewNode(originalChildNode, valueSelector);
        }

        return new ReadOnlyByteSpanTree<TNewValue>.ChildNodeInfo(
            newChildNodeArray,
            originalChildInfo.findByteIndex
        );
    }
    
    #endregion


    #region Instance

    readonly ChildNodeInfo rootChildNodeInfo;
    public readonly int valueCount;
    public readonly int maxNodeDepth;

    ReadOnlyByteSpanTree(ChildNodeInfo rootChildNodeInfo, int valueCount, int maxNodeDepth)
    {
        this.rootChildNodeInfo = rootChildNodeInfo;
        this.valueCount = valueCount;
        this.maxNodeDepth = maxNodeDepth;
    }

    public TValue GetValue(ReadOnlySpan<byte> key)
    {
        if (TryGetValue(key, out TValue? value))
        {
            return value;
        }
        else
        {
            // 값을 찾을 수 없음 -> 예외 발생
            throw new KeyNotFoundException($"'{Encoding.ASCII.GetString(key)}' 키를 찾을 수 없습니다.");
        }
    }
    public bool TryGetValue(ReadOnlySpan<byte> key, [NotNullWhen(true)] out TValue? value)
    {
        Node? findNode;
        if (rootChildNodeInfo.TryFindChildNode(key[0], out findNode) == false)
        {
            // 자식 노드를 찾을 수 없음
            value = default;
            return false;
        }

        for (int i = 1; i < key.Length; i++)
        {
            if (findNode.childNodeInfo == null)
            {
                // 자식 노드가 없음
                value = default;
                return false;
            }

            byte keyByte = key[i];
            
            if (findNode.childNodeInfo.TryFindChildNode(keyByte, out findNode))
            {
                // 자식 노드를 찾음 -> 다음 노드로 이동
                continue;
            }
            else
            {
                // 자식 노드를 찾을 수 없음
                value = default;
                return false;
            }
        }
        if (findNode.hasValue)
        {
            if (findNode.value == null)
            {
                // 값이 없음
                value = default;
                return false;
            }

            value = findNode.value;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    public ValueIterator GetValueIterator()
        => new ValueIterator(this);
    unsafe public void CopyTo(TValue* valueArray, int startIndex)
    {
        using (ValueIterator valueIterator = GetValueIterator())
        {
            for (int i = 0; i < valueCount; i++)
            {
                TValue? value;
                if (valueIterator.MoveNext(out value))
                {
                    valueArray[startIndex + i] = value;
                }
                else
                {
                    // 값이 없음 -> 예외 발생
                    throw new InvalidOperationException("값을 찾을 수 없습니다.");
                }
            }
        }
    }
    public TValue[] ToArray()
    {
        TValue[] valueArray = new TValue[valueCount];

        using (ValueIterator valueIterator = GetValueIterator())
        {
            for (int i = 0; i < valueCount; i++)
            {
                TValue? value;
                if (valueIterator.MoveNext(out value))
                {
                    valueArray[i] = value;
                }
                else
                {
                    // 값이 없음 -> 예외 발생
                    throw new InvalidOperationException("값을 찾을 수 없습니다.");
                }
            }
        }

        return valueArray;
    }
    public ReadOnlyByteSpanTree<TNewValue> CreateNewTree<TNewValue>(Func<TValue, TNewValue> valueSelector)
    {
        ReadOnlyByteSpanTree<TNewValue>.ChildNodeInfo newTreeChildNodeInfo = CreateNewChildNodeInfo(
            rootChildNodeInfo,
            valueSelector
        );

        return new ReadOnlyByteSpanTree<TNewValue>(newTreeChildNodeInfo, valueCount, maxNodeDepth);
    }

    NodeIterator GetNodeIterator()
        => new NodeIterator(this);

    #endregion
}