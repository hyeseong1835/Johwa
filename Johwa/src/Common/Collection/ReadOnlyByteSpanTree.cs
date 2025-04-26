using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace Johwa.Common.Collection;

public class ReadOnlyByteSpanTree<TValue>
{
    #region Object

    public ref struct Builder
    {
        #region Object
        
        struct ByteNode : IDisposable
        {
            #region Object

            #pragma warning disable CS8500 // 주소를 가져오거나, 크기를 가져오거나, 관리되는 형식에 대한 포인터를 선언합니다.

            /// <summary>
            /// ByteNode를 저장하는 단방향 링크드 리스트
            /// </summary>
            unsafe public struct List : IDisposable
            {
                #region Object

                /// <summary>
                /// 리스트를 순회하는 반복자  <br/>
                /// <br/>
                /// 절대 List가 해제된 이후 사용하지 마세요
                /// </summary>
                unsafe public struct Iterator
                {
                    ListNode* nodePtr;

                    public ref ByteNode Current { get {
                        if (nodePtr == null) 
                            throw new InvalidOperationException("현재 노드가 없습니다.");
                        
                        return ref (nodePtr->byteNode);
                    } }

                    public Iterator(List byteNodeList)
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
                ListNode* headNodePtr;
                int count;

                public ref ByteNode HeadByteNode => ref (headNodePtr->byteNode);
                public int Count => count;
                public bool IsEmpty => (headNodePtr == null);


                // Constructor
                public List()
                {
                    headNodePtr = null;
                    count = 0;
                }


                #region Method

                /// <summary>
                /// 노드를 앞부터 탐색해 값이 존재한다면 ByteNode의 참조를 반환하고 없다면 노드를 새로 생성하고 반환합니다.
                /// </summary>
                /// <param name="keyByte"></param>
                /// <returns></returns>
                public ref ByteNode GetOrCreateByteNode(byte keyByte)
                {
                    // 헤드 노드가 없음 -> 헤드 노드를 생성 후 반환
                    if (headNodePtr == null)
                    {
                        headNodePtr = ListNode.Create(keyByte, null, null);

                        // 개수 업데이트
                        count = 1;

                        // 반환
                        return ref (headNodePtr->byteNode);
                    }

                    ListNode* findNodePtr = headNodePtr;
                    ListNode* prevNodePtr = null;

                    while (findNodePtr != null)
                    {
                        byte findKeyByte = findNodePtr->byteNode.keyByte;

                        // 일치하는 키를 찾음 -> findNode의 바이트 노드를 반환
                        if (findKeyByte == keyByte)
                        {
                            return ref (findNodePtr->byteNode);
                        }
                        
                        // 아직 찾지 못함 -> 다음 노드로 이동
                        if (findKeyByte < keyByte)
                        {
                            prevNodePtr = findNodePtr;
                            findNodePtr = findNodePtr->next;
                            continue;
                        }

                        // 키를 찾지 못함 -> 찾기 루프 탈출
                        break;
                    }

                    // 새 리스트 노드 생성
                    ListNode* newNodePtr = ListNode.Create(keyByte, prevNodePtr, findNodePtr);

                    // 개수 업데이트
                    count++;

                    // 반환
                    return ref prevNodePtr->byteNode;
                }
                
                public Iterator GetIterator()
                    => new Iterator(this);
                
                public void Dispose()
                {
                    ListNode* findNodePtr = headNodePtr;
                    ListNode* nextNodePtr = null;

                    while (findNodePtr != null)
                    {
                        // 찾은 노드의 자식 리스트 Dispose
                        findNodePtr->byteNode.Dispose();

                        // 다음 노드 저장
                        nextNodePtr = findNodePtr->next;
                        
                        // 찾은 노드 해제
                        Marshal.FreeHGlobal((IntPtr)findNodePtr);

                        // 다음 노드로 이동
                        findNodePtr = nextNodePtr;
                    }
                }
                
                #endregion

                #endregion
            }

            /// <summary>
            /// ByteNode를 저장하는 단방향 링크드 리스트의 노드
            /// </summary>
            unsafe struct ListNode : IDisposable
            {
                public static ListNode* Create(byte keyByte, ListNode* prevNodePtr, ListNode* nextNodePtr)
                {
                    ListNode* newNodePtr = (ListNode*)Marshal.AllocHGlobal(sizeof(ListNode));

                    // prev -> new
                    if (prevNodePtr != null)
                    {
                        prevNodePtr->next = newNodePtr;
                    }

                    *newNodePtr = new ListNode(
                        new ByteNode(keyByte),
                        nextNodePtr
                    );

                    // new -> find
                    return newNodePtr;
                }

                public ByteNode byteNode;
                public ListNode* next;

                ListNode(ByteNode byteNode, ListNode* next)
                {
                    this.byteNode = byteNode;
                    this.next = next;
                }
                public void Dispose()
                {
                    byteNode.Dispose();
                }
            }

            #pragma warning restore CS8500

            #endregion


            #region Instance

            public readonly byte keyByte;
            public readonly ByteNode.List childList;

            TValue? value;
            bool hasValue;

            public TValue Value => value!;
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

            internal bool TryGetValue([NotNullWhen(true)] out TValue? value)
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

            public void Dispose()
            {
                childList.Dispose();
            }
            
            #endregion
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

        /// <summary>
        /// 값을 가지지않은 트리 생성
        /// </summary>
        /// <returns></returns>
        static ReadOnlyByteSpanTree<TValue> CreateEmptyTree()
        {
            return new ReadOnlyByteSpanTree<TValue>(
                null, 
                0, 
                1
            );
        }
        
        /// <summary>
        /// 단 하나의 값을 가지는 트리 생성
        /// </summary>
        /// <param name="byteNode"></param>
        /// <param name="value"></param>
        /// <param name="byteIndex"></param>
        /// <returns></returns>
        static ReadOnlyByteSpanTree<TValue> CreateValueTree(TValue value)
        {
            return new ReadOnlyByteSpanTree<TValue>(
                CreateValueTreeChildNodeInfo(
                    value
                ), 
                1, 
                1
            );
        }

        /// <summary>
        /// 둘 이상의 자식만 가지는 트리
        /// </summary>
        /// <param name="byteNodeList"></param>
        /// <param name="childFindByteIndex"></param>
        /// <returns></returns>
        static ReadOnlyByteSpanTree<TValue> CreateBranchTree(int childFindByteIndex, ByteNode.List byteNodeList)
        {
            BuildData buildData = new ();
            ParentNodeInfo treeRootInfo = ParentNodeInfo.CreateTreeRootInfo(childFindByteIndex);

            Node[] childNodeArray 
                = Builder.CreateNodeArray(
                    treeRootInfo,
                    byteNodeList, 
                    ref buildData
                );

            return new ReadOnlyByteSpanTree<TValue>(
                CreateBranchTreeChildNodeInfo(
                    childFindByteIndex,
                    childNodeArray
                ), 
                buildData.valueCount, 
                buildData.maxNodeDepth
            );
        }
        
        /// <summary>
        /// 값을 가지며 동시에 자식도 갖는 트리 생성
        /// </summary>
        /// <param name="byteNode"></param>
        /// <param name="value"></param>
        /// <param name="byteNodeList"></param>
        /// <param name="childFindByteIndex"></param>
        /// <returns></returns>
        static ReadOnlyByteSpanTree<TValue> CreateValueBranchTree(TValue value, int childFindByteIndex, ByteNode.List byteNodeList)
        {
            BuildData buildData = new();

            Node[] childNodeArray 
                = Builder.CreateNodeArray(
                    ParentNodeInfo.CreateTreeRootInfo(childFindByteIndex),
                    byteNodeList, 
                    ref buildData
                );

            return new ReadOnlyByteSpanTree<TValue>(
                CreateValueBranchTreeChildNodeInfo(
                    in value,
                    childFindByteIndex, 
                    childNodeArray
                ), 
                buildData.valueCount, 
                buildData.maxNodeDepth
            );
        }

        #endregion

        #region 노드 생성

        /// <summary>
        /// 값 노드 생성
        /// </summary>
        /// <param name="value"></param>
        static Node CreateValueNode(byte keyByte, TValue value, int nodeDepth, ref BuildData buildData)
        {
            buildData.valueCount++;
            buildData.maxNodeDepth = Math.Max(buildData.maxNodeDepth, nodeDepth);

            return ReadOnlyByteSpanTree<TValue>.CreateValueNode(
                buildData.nodeCount++, 
                keyByte, 
                value
            );
        }

        /// <summary>
        /// 브랜치 노드 생성
        /// </summary>
        /// <param name="keyByte"></param>
        /// <param name="childNodeInfo"></param>
        static Node CreateBranchNode(byte keyByte, ChildNodeInfo childNodeInfo, ref BuildData buildData)
        {
            return ReadOnlyByteSpanTree<TValue>.CreateBranchNode(
                buildData.nodeCount++, 
                keyByte, 
                childNodeInfo
            );
        }

        /// <summary>
        /// 값 브랜치 노드 생성
        /// </summary>
        /// <param name="keyByte"></param>
        /// <param name="childNodeInfo"></param>
        /// <param name="value"></param>
        static Node CreateValueBranchNode(byte keyByte, TValue value, ChildNodeInfo childNodeInfo, ref BuildData buildData)
        {
            buildData.valueCount++;

            return ReadOnlyByteSpanTree<TValue>.CreateValueBranchNode(
                buildData.nodeCount++, 
                keyByte, 
                value, 
                childNodeInfo
            );
        }

        #endregion

        #region 재귀적 노드 생성

        static Node CreateNode(ByteNode startNode, int byteIndex, int nodeDepth, ref BuildData buildData)
        {
            int moveCount;
            ref ByteNode byteNode = ref Builder.GetNextValueOrBranchNode(ref startNode, out moveCount);

            // 자식이 없음 -> 값 노드
            if (byteNode.childList.IsEmpty)
            {
                if (byteNode.HasValue)
                {
                    return Builder.CreateValueNode(
                        startNode.keyByte,
                        byteNode.Value,
                        nodeDepth,
                        ref buildData
                    );
                }
                else
                {
                    throw new InvalidOperationException("잘못된 노드가 존재합니다.");
                }
            }
            // 자식이 있음 -> 브랜치 노드
            else
            {
                ParentNodeInfo byteNodeInfo = new ParentNodeInfo(
                    byteIndex + moveCount,
                    nodeDepth
                );

                ChildNodeInfo childNodeInfo = CreateChildNodeInfo(
                    byteNodeInfo, 
                    byteNode.childList, 
                    ref buildData
                );
                
                TValue? value;
                if (byteNode.TryGetValue(out value))
                {
                    return Builder.CreateValueBranchNode(
                        startNode.keyByte,
                        value,
                        childNodeInfo,
                        ref buildData
                    );
                }
                else
                {
                    return Builder.CreateBranchNode(
                        startNode.keyByte,
                        childNodeInfo,
                        ref buildData
                    );
                }
            }
        }
        
        static Node[] CreateNodeArray(ParentNodeInfo parentNodeInfo, ByteNode.List byteNodeList, ref BuildData buildData)
        {
            Node[] childArray = new Node[byteNodeList.Count];

            ByteNode.List.Iterator byteNodeIterator = byteNodeList.GetIterator();

            int childNodeDepth = parentNodeInfo.nodeDepth + 1;

            for (int i = 0; i < byteNodeList.Count; i++)
            {
                byteNodeIterator.MoveNext();
                ref ByteNode byteNode = ref byteNodeIterator.Current;

                // 자식 노드 생성
                childArray[i] = CreateNode(
                    byteNode,
                    parentNodeInfo.childFindByteIndex, 
                    childNodeDepth, 
                    ref buildData
                );
            }

            return childArray;
        }

        static ChildNodeInfo CreateChildNodeInfo(ParentNodeInfo parentNodeInfo, ByteNode.List childByteNodeList, ref BuildData buildData)
        {
            Node[] childNodeArray = Builder.CreateNodeArray(
                parentNodeInfo,
                childByteNodeList,
                ref buildData
            );

            return new ChildNodeInfo(
                parentNodeInfo.childFindByteIndex,
                childNodeArray
            );
        }
        
        #endregion

        /// <summary>
        /// startNode를 포함하여 다음에 오는 값을 가지거나 여러 노드를 자식으로 갖는 노드를 얻습니다.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="moveCount"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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

        ByteNode.List byteNodeList;

        #endregion


        // 생성자
        public Builder()
        {
            byteNodeList = new();
        }


        #region 메서드

        public void Add(ReadOnlySpan<byte> key, TValue value)
        {
            if (key.Length == 0)
            {
                throw new ArgumentException("키의 길이는 0보다 길어야 합니다.");
            }

            byte keyByte = key[0];

            ref ByteNode findNode = ref byteNodeList.GetOrCreateByteNode(keyByte);

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
            // 바이트 노드가 없음 -> 빈 트리 반환
            if (byteNodeList.IsEmpty)
            {
                return Builder.CreateEmptyTree();
            }

            // 깊이가 1인 바이트 노드가 1개임
            if (byteNodeList.Count == 1)
            {
                int moveCount;
                ref ByteNode startNode = ref byteNodeList.HeadByteNode;
                ref ByteNode byteNode = ref Builder.GetNextValueOrBranchNode(ref startNode, out moveCount);

                if (byteNode.TryGetValue(out TValue? value))
                {
                    if (byteNode.childList.IsEmpty)
                    {
                        // 단 하나의 값을 가지는 트리 생성
                        return Builder.CreateValueTree(
                            value
                        );
                    }
                    else
                    {
                        // 값을 가지며 동시에 여러 자식을 갖는 트리 생성
                        return Builder.CreateValueBranchTree(
                            value, 
                            moveCount,
                            byteNode.childList
                        );
                    }
                }
                else
                {
                    if (byteNode.childList.IsEmpty)
                    {
                        throw new InvalidOperationException("잘못된 바이트 노드가 존재합니다.");
                    }
                    else
                    {
                        // 값을 가지며 동시에 여러 자식을 갖는 트리 생성
                        return Builder.CreateBranchTree(
                            moveCount,
                            byteNode.childList
                        );
                    }
                }
            }
            // 깊이가 1인 바이트 노드가 2개 이상임
            else
            {
                return Builder.CreateBranchTree(
                    0,
                    byteNodeList
                );
            }
        }
        
        public void Dispose()
        {
            // 노드 리스트를 Dispose
            byteNodeList.Dispose();
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
        public readonly int findByteIndex;
        public readonly Node[] childNodeArray;


        public ChildNodeInfo(int findByteIndex, Node[] childNodeArray)
        {
            this.findByteIndex = findByteIndex;
            this.childNodeArray = childNodeArray;
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

    struct ParentNodeInfo
    {
        public int childFindByteIndex;
        public int nodeDepth;

        public ParentNodeInfo(int childFindByteIndex, int nodeDepth)
        {
            this.childFindByteIndex = childFindByteIndex;
            this.nodeDepth = nodeDepth;
        }
        public static ParentNodeInfo CreateTreeRootInfo(int childFindByteIndex)
        {
            return new ParentNodeInfo(
                childFindByteIndex,
                0
            );
        }
    }

    struct NodeIterator : IDisposable
    {
        #region Object

        #pragma warning disable CS8500 // 주소를 가져오거나, 크기를 가져오거나, 관리되는 형식에 대한 포인터를 선언합니다.

        unsafe struct NodeFindInfo : IDisposable
        {
            public readonly GCHandle childNodeInfoGCHandle;
            readonly ChildNodeInfo* childNodeInfoPtr;
            public ChildNodeInfo ChildNodeInfo => *childNodeInfoPtr;
            public int childNodeFindIndex;

            public NodeFindInfo(ChildNodeInfo? childNodeInfo)
            {
                childNodeInfoGCHandle = GCHandle.Alloc(childNodeInfo, GCHandleType.Pinned);
                this.childNodeInfoPtr = &childNodeInfo;
                this.childNodeFindIndex = 0;
            }

            #region Method

            public void Dispose()
            {
                childNodeInfoGCHandle.Free();
            }


            public bool TryGetNode([NotNullWhen(true)] out Node? node)
            {
                if (childNodeFindIndex == -1) {
                    node = null;
                    return false;
                }

                Node[] childNodeArray = childNodeInfoPtr->childNodeArray;

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
                if (childNodeFindIndex == -1) {
                    node = null;
                    return false;
                }

                Node[] childNodeArray = childNodeInfoPtr->childNodeArray;

                if (childNodeFindIndex < childNodeArray.Length)
                {
                    node = childNodeArray[childNodeFindIndex++];
                    return true;
                }
                else
                {
                    node = null;
                    return false;
                }
            }

            #endregion
        }
            
        #pragma warning restore CS8500
        
        #endregion


        #region Instance

        Node? current;

        UnmanagedStack<NodeFindInfo> nodeFindInfoStack;

        public Node? Current => current;
        

        // 생성자
        public NodeIterator(ReadOnlyByteSpanTree<TValue> tree)
        {
            nodeFindInfoStack = new UnmanagedStack<NodeFindInfo>(tree.maxNodeDepth);

            NodeFindInfo rootNodeFindInfo = new NodeFindInfo(tree.rootChildNodeInfo);
            PushNodeFindInfo(in rootNodeFindInfo);
        }


        #region Method

        public void Dispose()
        {
            while (nodeFindInfoStack.IsEmpty == false)
            {
                nodeFindInfoStack.Pop().Dispose();
            }
            nodeFindInfoStack.Dispose();
        }


        void PushNodeFindInfo(in NodeFindInfo nodeFindInfo)
        {
            ChildNodeInfo? findChildNodeInfo = nodeFindInfo.ChildNodeInfo;
            while (findChildNodeInfo != null)
            {
                nodeFindInfoStack.Push(new NodeFindInfo(findChildNodeInfo));

                // 아래 첫번째 노드의 자식 정보로 이동
                findChildNodeInfo = findChildNodeInfo.childNodeArray[0].childNodeInfo;
            }
        }
        public bool MoveNext()
        {
            ref NodeFindInfo nodeFindInfo = ref nodeFindInfoStack.Peek();

            while (nodeFindInfoStack.IsEmpty == false)
            {
                ChildNodeInfo childNodeInfo = nodeFindInfo.ChildNodeInfo;
                if (nodeFindInfo.childNodeFindIndex < childNodeInfo.childNodeArray.Length)
                {
                    current = childNodeInfo.childNodeArray[nodeFindInfo.childNodeFindIndex++];
                    return true;
                }
                
                nodeFindInfo = ref nodeFindInfoStack.Pop();
            }
            
            // 모든 노드 탐색 정보 스택 소모 -> false 반환
            current = null;
            return false;
        }
        public bool MoveNext([NotNullWhen(true)] out Node? current)
        {
            ref NodeFindInfo nodeFindInfo = ref nodeFindInfoStack.Peek();

            while (nodeFindInfoStack.IsEmpty == false)
            {
                ChildNodeInfo childNodeInfo = nodeFindInfo.ChildNodeInfo;
                if (nodeFindInfo.childNodeFindIndex < childNodeInfo.childNodeArray.Length)
                {
                    this.current = childNodeInfo.childNodeArray[nodeFindInfo.childNodeFindIndex++];
                    current = this.current;
                    return true;
                }
                
                nodeFindInfo = ref nodeFindInfoStack.Pop();
            }
            
            // 모든 노드 탐색 정보 스택 소모 -> false 반환
            this.current = null;
            current = this.current;
            return false;
        }
        
        #endregion

        #endregion
    }
    
    #endregion


    #region Static

    #region 노드 생성

    /// <summary>
    /// 값만 가진 노드 생성
    /// </summary>
    /// <param name="value"></param>
    static Node CreateValueNode(int nodeIndex, byte keyByte, in TValue value)
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
    /// 둘 이상의 자식 노드만 가진 노드 생성
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
    /// 값과 하나 이상의 자식 노드를 모두 가지는 노드 생성
    /// </summary>
    /// <param name="keyByte"></param>
    /// <param name="childNodeInfo"></param>
    /// <param name="value"></param>
    static Node CreateValueBranchNode(int nodeIndex, byte keyByte, TValue value, ChildNodeInfo childNodeInfo)
    {
        return new Node(
            nodeIndex,
            keyByte,
            value,
            true,
            childNodeInfo
        );
    }

    #endregion

    #region 루트 자식 정보 생성

    static ChildNodeInfo CreateValueTreeChildNodeInfo(TValue value)
    {
        Node valueNode 
            = ReadOnlyByteSpanTree<TValue>.CreateValueNode(
                0, 
                default, 
                value
            );

        return new ChildNodeInfo(
            default,
            [ valueNode ]
        );
    }
    static ChildNodeInfo CreateBranchTreeChildNodeInfo(int childFindByteIndex, Node[] childNodeArray)
    {
        return new ChildNodeInfo(
            childFindByteIndex,
            childNodeArray
        );
    }
    static ChildNodeInfo CreateValueBranchTreeChildNodeInfo(in TValue value, int childFindByteIndex, Node[] childNodeArray)
    {
        ChildNodeInfo valueNodeChildInfo 
            = new ChildNodeInfo(
                childFindByteIndex,
                childNodeArray
            );

        Node valueNode 
            = ReadOnlyByteSpanTree<TValue>.CreateValueBranchNode(
                0, 
                default, 
                value,
                valueNodeChildInfo
            );

        return new ChildNodeInfo(
            default,
            [ valueNode ]
        );
    }

    #endregion

    #region 다른 타입의 값을 가지는 트리로 구조 복사

    static ReadOnlyByteSpanTree<TNewValue>.Node CreateNewNode<TNewValue>(Node originalNode, Func<TValue, TNewValue> valueSelector)
    {
        //값이 있음
        if (originalNode.TryGetValue(out TValue? originalValue))
        {
            TNewValue newValue = valueSelector.Invoke(originalValue);

            if (originalNode.childNodeInfo == null)
            {
                // 값 노드 생성
                return ReadOnlyByteSpanTree<TNewValue>.CreateValueNode(
                    originalNode.nodeIndex,
                    originalNode.keyByte,
                    newValue
                );
            }
            else
            {
                ReadOnlyByteSpanTree<TNewValue>.ChildNodeInfo newChildNodeInfo 
                    = CreateNewChildNodeInfo(originalNode.childNodeInfo, valueSelector);

                // 값 브랜치 노드 생성
                return ReadOnlyByteSpanTree<TNewValue>.CreateValueBranchNode(
                    originalNode.nodeIndex,
                    originalNode.keyByte,
                    newValue, 
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
                    originalNode.nodeIndex,
                    originalNode.keyByte,
                    newChildNodeInfo
                );
            }
        }
    }
    
    static ReadOnlyByteSpanTree<TNewValue>.ChildNodeInfo CreateNewChildNodeInfo<TNewValue>(ChildNodeInfo originalChildInfo, Func<TValue, TNewValue> valueSelector)
    {
        // 새 자식 노드 배열 생성
        ReadOnlyByteSpanTree<TNewValue>.Node[] newChildNodeArray = new ReadOnlyByteSpanTree<TNewValue>.Node[originalChildInfo.childNodeArray.Length];
        for (int i = 0; i < newChildNodeArray.Length; i++)
        {
            Node originalChildNode = originalChildInfo.childNodeArray[i];
            newChildNodeArray[i] = CreateNewNode(originalChildNode, valueSelector);
        }

        // 자식 정보 생성
        return new ReadOnlyByteSpanTree<TNewValue>.ChildNodeInfo(
            originalChildInfo.findByteIndex,
            newChildNodeArray
        );
    }
    
    #endregion

    #endregion


    #region Instance

    readonly ChildNodeInfo? rootChildNodeInfo;
    public readonly int valueCount;
    public readonly int maxNodeDepth;

    ReadOnlyByteSpanTree(ChildNodeInfo? rootChildNodeInfo, int valueCount, int maxNodeDepth)
    {
        this.rootChildNodeInfo = rootChildNodeInfo;
        this.valueCount = valueCount;
        this.maxNodeDepth = maxNodeDepth;
    }


    #region Method

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
        // 키의 길이가 0임 -> false 반환
        if (key.Length == 0)
        {
            value = default;
            return false;
        }

        // 트리가 비어있음 -> false 반환
        if (rootChildNodeInfo == null)
        {
            value = default;
            return false;
        }

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
            value = findNode.value!;
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
    public TValue[] ToArray()
    {
        TValue[] valueArray = new TValue[valueCount];

        using (NodeIterator nodeIterator = GetNodeIterator())
        {
            for (int i = 0; i < valueCount; i++)
            {
                Node? currentNode;
                while (nodeIterator.MoveNext(out currentNode))
                {
                    // 찾은 노드가 값이 있음 -> 값 설정후 다음 배열의 빈 공간으로 이동
                    if (currentNode.hasValue)
                    {
                        // 찾은 노드가 값이 있음
                        valueArray[i] = currentNode.value!;
                        break;
                    }
                }
            }
        }

        return valueArray;
    }
    public ReadOnlyByteSpanTree<TNewValue> CreateNewTree<TNewValue>(Func<TValue, TNewValue> valueSelector)
    {
        ReadOnlyByteSpanTree<TNewValue>.ChildNodeInfo? newTreeChildNodeInfo;
        if (rootChildNodeInfo == null) 
        {
            newTreeChildNodeInfo = null;
        }
        else 
        {
            newTreeChildNodeInfo = CreateNewChildNodeInfo(
                rootChildNodeInfo,
                valueSelector
            );
        }

        return new ReadOnlyByteSpanTree<TNewValue>(newTreeChildNodeInfo, valueCount, maxNodeDepth);
    }

    NodeIterator GetNodeIterator()
        => new NodeIterator(this);

    #endregion

    #endregion
}