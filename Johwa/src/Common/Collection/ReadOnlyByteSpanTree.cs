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
            public BuildData(int nodeCount = 0, int valueCount = 0, int maxNodeDepth = 0)
            {
                this.nodeCount = nodeCount;
                this.valueCount = valueCount;
                this.maxNodeDepth = maxNodeDepth;
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
                CreateEmptyTreeRootNode(),
                0, 
                0
            );
        }
        
        /// <summary>
        /// 단 하나의 값을 가지는 트리 생성
        /// </summary>
        /// <param name="byteNode"></param>
        /// <param name="value"></param>
        /// <param name="byteIndex"></param>
        /// <returns></returns>
        static ReadOnlyByteSpanTree<TValue> CreateValueTree(byte[] keySlice, TValue value)
        {
            return new ReadOnlyByteSpanTree<TValue>(
                CreateValueTreeRootNode(
                    keySlice,
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
        static ReadOnlyByteSpanTree<TValue> CreateBranchTree(byte[] keySlice, ByteNode.List byteNodeList)
        {
            BuildData buildData = new (
                nodeCount: 1
            );

            Node[] childNodeArray 
                = Builder.CreateNodeArray(
                    1,
                    byteNodeList, 
                    ref buildData
                );

            return new ReadOnlyByteSpanTree<TValue>(
                CreateBranchTreeRootNode(
                    keySlice,
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
        static ReadOnlyByteSpanTree<TValue> CreateValueBranchTree(byte[] keySlice, TValue value, ByteNode.List byteNodeList)
        {
            BuildData buildData = new (
                nodeCount: 1
            );

            Node[] childNodeArray 
                = Builder.CreateNodeArray(
                    2,
                    byteNodeList, 
                    ref buildData
                );

            return new ReadOnlyByteSpanTree<TValue>(
                CreateValueBranchTreeRootNode(
                    keySlice,
                    value,
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
        static Node CreateValueNode(byte[] keySlice, TValue value, int nodeDepth, ref BuildData buildData)
        {
            buildData.maxNodeDepth = Math.Max(buildData.maxNodeDepth, nodeDepth);

            return ReadOnlyByteSpanTree<TValue>.CreateValueNode(
                buildData.nodeCount++, 
                keySlice, 
                new ValueInfo(value, buildData.valueCount++)
            );
        }

        /// <summary>
        /// 브랜치 노드 생성
        /// </summary>
        /// <param name="keyByte"></param>
        /// <param name="childNodeInfo"></param>
        static Node CreateBranchNode(byte[] keySlice, Node[] childNodeArray, ref BuildData buildData)
        {
            return ReadOnlyByteSpanTree<TValue>.CreateBranchNode(
                buildData.nodeCount++, 
                keySlice, 
                childNodeArray
            );
        }

        /// <summary>
        /// 값 브랜치 노드 생성
        /// </summary>
        /// <param name="keyByte"></param>
        /// <param name="childNodeInfo"></param>
        /// <param name="value"></param>
        static Node CreateValueBranchNode(byte[] keySlice, TValue value, Node[] childNodeArray, ref BuildData buildData)
        {
            return ReadOnlyByteSpanTree<TValue>.CreateValueBranchNode(
                buildData.nodeCount++, 
                keySlice, 
                new ValueInfo(value, buildData.valueCount++), 
                childNodeArray
            );
        }

        #endregion

        #region 재귀적 노드 생성

        static Node CreateNode(byte[] keySlice, ref ByteNode byteNode, int nodeDepth, ref BuildData buildData)
        {
            // 자식이 없음 -> 값 노드
            if (byteNode.childList.IsEmpty)
            {
                if (byteNode.TryGetValue(out TValue? value))
                {
                    return Builder.CreateValueNode(
                        keySlice,
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
            // 자식이 있음 -> 브랜치 노드
            else
            {
                Node[] childNodeArray = Builder.CreateNodeArray(
                    nodeDepth, 
                    byteNode.childList, 
                    ref buildData
                );
                
                // 값 브랜치 노드
                if (byteNode.TryGetValue(out TValue? value))
                {
                    return Builder.CreateValueBranchNode(
                        keySlice,
                        value,
                        childNodeArray,
                        ref buildData
                    );
                }
                // 브랜치노드
                else
                {
                    return Builder.CreateBranchNode(
                        keySlice,
                        childNodeArray,
                        ref buildData
                    );
                }
            }
        }
        
        static Node[] CreateNodeArray(int parentNodeDepth, ByteNode.List byteNodeList, ref BuildData buildData)
        {
            Node[] childArray = new Node[byteNodeList.Count];

            ByteNode.List.Iterator byteNodeIterator = byteNodeList.GetIterator();

            for (int i = 0; i < byteNodeList.Count; i++)
            {
                if (byteNodeIterator.MoveNext())
                {
                    ref ByteNode byteNode = ref byteNodeIterator.Current;

                    byte[] keySlice;
                    ref ByteNode nextByteNode = ref GetNextValueOrBranchNode(ref byteNode, out keySlice);

                    // 자식 노드 생성
                    childArray[i] = CreateNode(
                        keySlice,
                        ref nextByteNode,
                        parentNodeDepth + keySlice.Length, 
                        ref buildData
                    );
                }
                else
                {
                    throw new Exception("잘못된 노드가 있습니다.");
                }
            }

            return childArray;
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
        static ref ByteNode GetNextValueOrBranchNode(ref ByteNode startNode, out byte[] keySlice)
        {
            int moveCount;
            ref ByteNode byteNode = ref GetNextValueOrBranchNode(ref startNode, out moveCount);

            ref ByteNode findByteNode =ref startNode;
            keySlice = new byte[moveCount + 1];

            // keySlice 채우기
            keySlice[0] = startNode.keyByte;

            for (int i = 1; i < moveCount + 1; i++)
            {
                // 다음 노드로 이동
                findByteNode = ref findByteNode.childList.HeadByteNode;

                keySlice[i] = findByteNode.keyByte;
            }

            return ref byteNode;
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
            else
            {
                ref ByteNode startNode = ref byteNodeList.HeadByteNode;

                byte[] keySlice;
                ref ByteNode nextByteNode = ref Builder.GetNextValueOrBranchNode(ref startNode, out keySlice);

                // 첫번째 분기에 값이 있음
                if (nextByteNode.TryGetValue(out TValue? value))
                {
                    if (nextByteNode.childList.IsEmpty)
                    {
                        // 단 하나의 값을 가지는 트리 생성
                        return Builder.CreateValueTree(
                            keySlice,
                            value
                        );
                    }
                    else
                    {
                        // 값을 가지며 동시에 여러 자식을 갖는 트리 생성
                        return Builder.CreateValueBranchTree(
                            keySlice,
                            value, 
                            nextByteNode.childList
                        );
                    }
                }
                else
                {
                    if (nextByteNode.childList.IsEmpty)
                    {
                        throw new InvalidOperationException("잘못된 바이트 노드가 존재합니다.");
                    }
                    else
                    {
                        // 값을 가지며 동시에 여러 자식을 갖는 트리 생성
                        return Builder.CreateBranchTree(
                            keySlice,
                            nextByteNode.childList
                        );
                    }
                }
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
    }

    unsafe public struct ConvertedByteSpanTree<TConvertedValue> : IDisposable
        where TConvertedValue: unmanaged
    {
        ReadOnlyByteSpanTree<TValue> originalTree;
        GCHandle originalTreeGCHandle;

        TConvertedValue* valueArray;
        Func<TValue, TConvertedValue> valueConverter;

        public ConvertedByteSpanTree(ReadOnlyByteSpanTree<TValue> originalTree, Func<TValue, TConvertedValue> valueConverter)
        {
            this.originalTree = originalTree;
            originalTreeGCHandle = GCHandle.Alloc(originalTree, GCHandleType.Pinned);

            this.valueConverter = valueConverter;

            valueArray = (TConvertedValue*)Marshal.AllocHGlobal(sizeof(TConvertedValue) * originalTree.valueCount);
            ReadOnlyByteSpanTree<TValue>.ValueIterator valueIterator = originalTree.GetValueIterator();
            for (int i = 0; i < originalTree.valueCount; i++)
            {
                TValue? originalValue;
                if (valueIterator.MoveNext(out originalValue))
                {
                    valueArray[i] = valueConverter.Invoke(originalValue);
                }
                else
                {
                    throw new InvalidOperationException("원본 트리의 값 수가 일치하지 않습니다.");
                }
            }
        }
        public TConvertedValue GetValue(ReadOnlySpan<byte> key)
        {
            Node originalNode = originalTree.GetNode(key);
            return valueArray[originalNode.valueIndex];
        }

        public void Dispose()
        {
            originalTreeGCHandle.Free();

            Marshal.FreeHGlobal((IntPtr)valueArray);
            valueArray = null;
        }
    }
    
    internal class Node
    {
        public int nodeIndex;
        public int valueIndex;
        public readonly byte[] keySlice;

        public readonly ValueInfo? valueInfo;

        Node? parentNode;
        public Node? ParentNode => parentNode;
        public readonly Node[]? childNodeArray;


        #region Constructor

        public Node(int nodeIndex, byte[] keySlice, ValueInfo? valueInfo, Node[]? childNodeArray)
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
        }

        #endregion

        /// <summary>
        /// 이진 탐색으로 자식 노드를 찾음
        /// </summary>
        /// <param name="keySlice">찾을 keySlice</param>
        /// <param name="childNode">찾은 노드</param>
        /// <returns>지정한 keyByte를 가진 노드의 존재 여부</returns>
        public bool TryFindChildNode(byte keySliceStartByte, [NotNullWhen(true)] out Node? childNode)
        {
            if (childNodeArray == null) {
                childNode = null;
                return false;
            }

            int minIndex = 0;
            int maxIndex = childNodeArray.Length - 1;

            while (minIndex <= maxIndex)
            {
                int midIndex = (minIndex + maxIndex) / 2;
                Node midNode = childNodeArray[midIndex];
                byte midNodeKeySliceStart = midNode.keySlice[0];

                // 가운데 노드 == 찾을 노드
                if (midNodeKeySliceStart == keySliceStartByte)
                {
                    childNode = midNode;
                    return true;
                }
                // 가운데 노드 < 찾을 노드
                else if (midNodeKeySliceStart < keySliceStartByte)
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


    #region Static

    #region 노드 생성

    /// <summary>
    /// 값만 가진 노드 생성
    /// </summary>
    /// <param name="value"></param>
    static Node CreateValueNode(int nodeIndex, byte[] keySlice, ValueInfo valueInfo)
    {
        return new Node(
            nodeIndex,
            keySlice,
            valueInfo,
            null
        );
    }

    /// <summary>
    /// 둘 이상의 자식 노드만 가진 노드 생성
    /// </summary>
    /// <param name="keySlice"></param>
    /// <param name="childNodeArray"></param>
    static Node CreateBranchNode(int nodeIndex, byte[] keySlice, Node[] childNodeArray)
    {
        return new Node(
            nodeIndex,
            keySlice,
            null,
            childNodeArray
        );
    }

    /// <summary>
    /// 값과 하나 이상의 자식 노드를 모두 가지는 노드 생성
    /// </summary>
    /// <param name="keyByte"></param>
    /// <param name="childNodeInfo"></param>
    /// <param name="value"></param>
    static Node CreateValueBranchNode(int nodeIndex, byte[] keySlice, ValueInfo valueInfo, Node[] childNodeArray)
    {
        return new Node(
            nodeIndex,
            keySlice,
            valueInfo,
            childNodeArray
        );
    }

    #endregion

    #region 루트 노드 생성

    static Node CreateEmptyTreeRootNode()
    {
        Node emptyNode
            = ReadOnlyByteSpanTree<TValue>.CreateBranchNode(
                0,
                new byte[0],
                new Node[0]
            );

        return emptyNode;
    }

    static Node CreateValueTreeRootNode(byte[] keySlice, TValue value)
    {
        Node valueNode 
            = ReadOnlyByteSpanTree<TValue>.CreateValueNode(
                0, 
                keySlice, 
                new ValueInfo(value, 0)
            );

        return valueNode;
    }

    static Node CreateBranchTreeRootNode(byte[] keySlice, Node[] childNodeArray)
    {
        Node branchNode
            = ReadOnlyByteSpanTree<TValue>.CreateBranchNode(
                0,
                keySlice,
                childNodeArray
            );

        return branchNode;
    }

    static Node CreateValueBranchTreeRootNode(byte[] keySlice, TValue value, Node[] childNodeArray)
    {
        Node valueBranchNode =
            ReadOnlyByteSpanTree<TValue>.CreateValueBranchNode(
                0, 
                keySlice,
                new ValueInfo(value, 0),
                childNodeArray
            );

        return valueBranchNode;
    }

    #endregion

    #region 다른 타입의 값을 가지는 트리로 구조 복사

    static ReadOnlyByteSpanTree<TNewValue>.Node CreateNewNode<TNewValue>(Node originalNode, Func<TValue, TNewValue> valueSelector)
    {
        //값이 없음
        if (originalNode.valueInfo == null)
        {
            if (originalNode.childNodeArray == null)
            {
                throw new ArgumentException("잘못된 노드가 존재합니다.");
            }
            else
            {
                // 브랜치 노드 생성
                ReadOnlyByteSpanTree<TNewValue>.Node[] newChildNodeArray
                    = CreateNewChildNodeArray(originalNode.childNodeArray, valueSelector);

                return ReadOnlyByteSpanTree<TNewValue>.CreateBranchNode(
                    originalNode.nodeIndex,
                    originalNode.keySlice,
                    newChildNodeArray
                );
            }
        }
        // 값 있음
        else
        {
            TNewValue newValue = valueSelector.Invoke(originalNode.valueInfo.value);
            ReadOnlyByteSpanTree<TNewValue>.ValueInfo newValueInfo 
                = new ReadOnlyByteSpanTree<TNewValue>.ValueInfo(
                    newValue, 
                    originalNode.valueInfo.valueIndex
                );

            // 자식 없음
            if (originalNode.childNodeArray == null)
            {
                // 값 노드 생성
                return ReadOnlyByteSpanTree<TNewValue>.CreateValueNode(
                    originalNode.nodeIndex,
                    originalNode.keySlice,
                    newValueInfo
                );
            }
            // 자식 있음
            else
            {
                ReadOnlyByteSpanTree<TNewValue>.Node[] newChildNodeArray
                    = CreateNewChildNodeArray(originalNode.childNodeArray, valueSelector);

                // 값 브랜치 노드 생성
                return ReadOnlyByteSpanTree<TNewValue>.CreateValueBranchNode(
                    originalNode.nodeIndex,
                    originalNode.keySlice,
                    newValueInfo, 
                    newChildNodeArray
                );
            }
        }
    }
    
    static ReadOnlyByteSpanTree<TNewValue>.Node[] CreateNewChildNodeArray<TNewValue>(Node[] originalChildArray, Func<TValue, TNewValue> valueSelector)
    {
        // 새 자식 노드 배열 생성
        ReadOnlyByteSpanTree<TNewValue>.Node[] newChildNodeArray = new ReadOnlyByteSpanTree<TNewValue>.Node[originalChildArray.Length];
        for (int i = 0; i < newChildNodeArray.Length; i++)
        {
            Node originalChildNode = originalChildArray[i];
            newChildNodeArray[i] = CreateNewNode(originalChildNode, valueSelector);
        }

        // 반환
        return newChildNodeArray;
    }
    
    #endregion

    #endregion


    #region Instance
    
    readonly Node rootNode;
    public readonly int valueCount;
    public readonly int maxNodeDepth;

    ReadOnlyByteSpanTree(Node rootNode, int valueCount, int maxNodeDepth)
    {
        this.rootNode = rootNode;
        this.valueCount = valueCount;
        this.maxNodeDepth = maxNodeDepth;
    }


    #region Method

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
    public ReadOnlyByteSpanTree<TNewValue> CreateNewTree<TNewValue>(Func<TValue, TNewValue> valueSelector)
    {
        return new ReadOnlyByteSpanTree<TNewValue>(
            CreateNewTreeNode(rootNode, valueSelector), 
            valueCount, 
            maxNodeDepth
        );
    }
    static ReadOnlyByteSpanTree<TNewValue>.Node CreateNewTreeNode<TNewValue>(Node originalNode, Func<TValue, TNewValue> valueSelector)
    {
        return new ReadOnlyByteSpanTree<TNewValue>.Node(
            originalNode.nodeIndex,
            originalNode.keySlice,
            CreateNewTreeNodeValueInfo(originalNode, valueSelector),
            CreateNewTreeNodeChildNodeArray(originalNode, valueSelector)
        );
    }
    static ReadOnlyByteSpanTree<TNewValue>.ValueInfo? CreateNewTreeNodeValueInfo<TNewValue>(Node originalNode, Func<TValue, TNewValue> valueSelector)
    {
        if (originalNode.valueInfo == null) 
        {
            return null;
        }
        else
        {
            return new ReadOnlyByteSpanTree<TNewValue>.ValueInfo(
                valueSelector.Invoke(originalNode.valueInfo.value),
                originalNode.valueInfo.valueIndex
            );
        }
    }
    static ReadOnlyByteSpanTree<TNewValue>.Node[]? CreateNewTreeNodeChildNodeArray<TNewValue>(Node originalNode, Func<TValue, TNewValue> valueSelector)
    {
        if (originalNode.childNodeArray == null)
        {
            return null;
        }
        else
        {
            ReadOnlyByteSpanTree<TNewValue>.Node[] newTreeNodeChildNodeArray 
                = new ReadOnlyByteSpanTree<TNewValue>.Node[originalNode.childNodeArray.Length];
                
            for (int i = 0; i < newTreeNodeChildNodeArray.Length; i++)
            {
                Node originalNodeChild = originalNode.childNodeArray[i];
                newTreeNodeChildNodeArray[i] = CreateNewTreeNode(originalNodeChild, valueSelector);
            }

            return newTreeNodeChildNodeArray;
        }
    }

    internal NodeIterator GetNodeIterator()
        => new NodeIterator(this);

    internal Node GetNode(ReadOnlySpan<byte> key)
    {
        int keyLength = key.Length;

        // 키의 길이가 0임 -> 오류
        if (key.Length == 0)
            throw new ArgumentException("키의 길이는 0보다 길어야 합니다.");

        Node? findNode = rootNode;
        int keyFindByteIndex = 0;
           
        while (keyFindByteIndex <= keyLength)
        {
            byte[] findNodeKeySlice = findNode.keySlice;
            int findNodeKeySliceLength = findNode.keySlice.Length;

            if (findNodeKeySliceLength == 0)
                throw new Exception("잘못된 노드가 존재합니다.");

            // 찾은 노드 키 구성 확인
            for (int i = 1; i < findNodeKeySliceLength; i++)
            {
                // 루트 자식 찾기
                byte keyByte = key[keyFindByteIndex++];
                byte findNodeKeyByte = findNodeKeySlice[i];

                // 찾은 노드와 키 구성이 다르면
                if (keyByte != findNodeKeyByte)
                    throw new KeyNotFoundException($"'{Encoding.ASCII.GetString(key)}' 키를 찾을 수 없습니다.");
            }

            // 키 전부 탐색 -> 찾은 노드 반환
            if (keyFindByteIndex == keyLength)
            {
                // 찾았다!
                return findNode;
            }

            byte childNodeKeySliceStartByte = key[keyFindByteIndex++];
            if (findNode.TryFindChildNode(childNodeKeySliceStartByte, out findNode))
            {
                // 찾은 노드로 이동
                continue;
            }
            else
            {
                // 자식 노드를 찾을 수 없음
                throw new KeyNotFoundException($"'{Encoding.ASCII.GetString(key)}' 키를 찾을 수 없습니다.");
            }
        }

        throw new KeyNotFoundException($"'{Encoding.ASCII.GetString(key)}' 키를 찾을 수 없습니다.");
    }

    internal bool TryGetNode(ReadOnlySpan<byte> key, [NotNullWhen(true)] out Node? node)
    {
        // 키의 길이가 0임 -> false 반환
        if (key.Length == 0) {
            node = null;
            return false;
        }

        Node? findNode = rootNode;
        int keyFindByteIndex = 0;
           
        while (keyFindByteIndex < key.Length)
        {
            // 루트 자식 찾기
            byte keyByte = key[keyFindByteIndex];

            if (findNode.TryFindChildNode(keyByte, out findNode))
            {
                keyFindByteIndex+;
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
}