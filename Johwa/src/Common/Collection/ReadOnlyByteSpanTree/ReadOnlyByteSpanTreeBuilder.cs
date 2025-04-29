using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace Johwa.Common.Collection;

public partial class ReadOnlyByteSpanTree<TValue>
{
    /// <summary>
    /// 내부는 참조 변수로 이루어져 있기 때문에 복사하여도 정보를 공유함
    /// </summary>
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
            public int maxBranchDepth;

            public BuildData()
            {
                this.nodeCount = 0;
                this.valueCount = 0;
                this.maxNodeDepth = 0;
                this.maxBranchDepth = 0;
            }
            public BuildData(int nodeCount = 0, int valueCount = 0, int maxNodeDepth = 0, int maxBranchDepth = 0)
            {
                this.nodeCount = nodeCount;
                this.valueCount = valueCount;
                this.maxNodeDepth = maxNodeDepth;
                this.maxBranchDepth = maxBranchDepth;
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
                1,
                0
            );
        }

        /// <summary>
        /// 둘 이상의 자식만 가지는 트리
        /// </summary>
        /// <param name="byteNodeList"></param>
        /// <param name="childFindByteIndex"></param>
        /// <returns></returns>
        static ReadOnlyByteSpanTree<TValue> CreateBranchTree(byte[] keySlice, ByteNode.List byteNodeList, int branchDepth)
        {
            BuildData buildData = new (
                nodeCount: 1
            );

            Node[] childNodeArray 
                = Builder.CreateNodeArray(
                    1,
                    branchDepth,
                    byteNodeList, 
                    ref buildData
                );

            return new ReadOnlyByteSpanTree<TValue>(
                CreateBranchTreeRootNode(
                    keySlice,
                    childNodeArray
                ),
                buildData.valueCount, 
                buildData.maxNodeDepth,
                buildData.maxBranchDepth
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
        static ReadOnlyByteSpanTree<TValue> CreateValueBranchTree(byte[] keySlice, TValue value, ByteNode.List byteNodeList, int branchDepth)
        {
            BuildData buildData = new (
                nodeCount: 1
            );

            Node[] childNodeArray 
                = Builder.CreateNodeArray(
                    2,
                    branchDepth,
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
                buildData.maxNodeDepth,
                buildData.maxBranchDepth
            );
        }

        #endregion

        #region 노드 생성

        /// <summary>
        /// 값 노드 생성
        /// </summary>
        /// <param name="value"></param>
        static Node CreateValueNode(byte[] keySlice, TValue value, int nodeDepth, int branchDepth, ref BuildData buildData)
        {
            buildData.maxNodeDepth = Math.Max(buildData.maxNodeDepth, nodeDepth);
            buildData.maxBranchDepth = Math.Max(buildData.maxBranchDepth, branchDepth);

            return ReadOnlyByteSpanTree<TValue>.CreateValueNode(
                buildData.nodeCount++, 
                keySlice, 
                new ValueInfo(value, buildData.valueCount++),
                branchDepth
            );
        }

        /// <summary>
        /// 브랜치 노드 생성
        /// </summary>
        /// <param name="keyByte"></param>
        /// <param name="childNodeInfo"></param>
        static Node CreateBranchNode(byte[] keySlice, Node[] childNodeArray, int branchDepth, ref BuildData buildData)
        {
            return ReadOnlyByteSpanTree<TValue>.CreateBranchNode(
                buildData.nodeCount++, 
                keySlice, 
                childNodeArray,
                branchDepth
            );
        }

        /// <summary>
        /// 값 브랜치 노드 생성
        /// </summary>
        /// <param name="keyByte"></param>
        /// <param name="childNodeInfo"></param>
        /// <param name="value"></param>
        static Node CreateValueBranchNode(byte[] keySlice, TValue value, Node[] childNodeArray, int branchDepth, ref BuildData buildData)
        {
            return ReadOnlyByteSpanTree<TValue>.CreateValueBranchNode(
                buildData.nodeCount++, 
                keySlice, 
                new ValueInfo(value, buildData.valueCount++), 
                childNodeArray,
                branchDepth
            );
        }

        #endregion

        #region 재귀적 노드 생성

        static Node CreateNode(byte[] keySlice, ref ByteNode byteNode, int nodeDepth, int branchDepth, ref BuildData buildData)
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
                        branchDepth,
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
                    branchDepth,
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
                        branchDepth,
                        ref buildData
                    );
                }
                // 브랜치노드
                else
                {
                    return Builder.CreateBranchNode(
                        keySlice,
                        childNodeArray,
                        branchDepth,
                        ref buildData
                    );
                }
            }
        }
        
        static Node[] CreateNodeArray(int parentNodeDepth, int parentBranchDepth, ByteNode.List byteNodeList, ref BuildData buildData)
        {
            int byteNodeListCount = byteNodeList.Count;
            
            if (byteNodeListCount == 0)
                throw new ArgumentException("byteNodeList가 비었습니다.");

            ByteNode.List.Iterator byteNodeIterator = byteNodeList.GetIterator();

            Node[] childArray = new Node[byteNodeListCount];

            // 자식 노드가 1개
            if (byteNodeListCount == 1)
            {
                if (byteNodeIterator.MoveNext())
                {
                    ref ByteNode byteNode = ref byteNodeIterator.Current;

                    byte[] keySlice;
                    ref ByteNode nextByteNode = ref GetNextValueOrBranchNode(ref byteNode, out keySlice);

                    // 자식 노드 생성
                    childArray[0] = CreateNode(
                        keySlice,
                        ref nextByteNode,
                        parentNodeDepth + keySlice.Length, 
                        parentBranchDepth,
                        ref buildData
                    );
                }
                else
                {
                    throw new Exception("잘못된 노드가 있습니다.");
                }
            }
            // 자식 노드가 2개 이상
            else
            {
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
                            parentBranchDepth + 1,
                            ref buildData
                        );
                    }
                    else
                    {
                        throw new Exception("잘못된 노드가 있습니다.");
                    }
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
            // 바이트 노드가 있음
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
                        // 값을 가지며 동시에 하나 이상의 자식을 갖는 트리 생성
                        return Builder.CreateValueBranchTree(
                            keySlice,
                            value, 
                            nextByteNode.childList,
                            1
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
                        // 둘 이상의 자식을 갖는 트리 생성
                        return Builder.CreateBranchTree(
                            keySlice,
                            nextByteNode.childList,
                            1
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
}