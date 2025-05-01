using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Johwa.Common.Collection;

/// <summary>
/// 내부는 참조 변수로 이루어져 있기 때문에 복사하여도 정보를 공유함
/// </summary>
public ref struct TempByteSpanTree<TValue>
    where TValue : unmanaged
{
    #region Object
    
    unsafe internal struct ByteNode : IDisposable
    {
        #region Object

        /// <summary>
        /// ByteNode를 저장하는 단방향 링크드 리스트 (비관리형)
        /// </summary>
        unsafe public struct List : IEnumerable<ByteNode>, IDisposable
        {
            #region Object

            /// <summary>
            /// 리스트를 순회하는 반복자  <br/>
            /// <br/>
            /// 절대 List가 해제된 이후 사용하지 마세요
            /// </summary>
            unsafe public struct Enumerator : IEnumerator<ByteNode>
            {
                #region Field & Property

                #region 명시적 인터페이스 구현

                ByteNode IEnumerator<ByteNode>.Current { get {
                    if (currentListNodePtr == null) 
                        throw new Exception("현재 노드가 없습니다.");
                    
                    return currentListNodePtr->byteNode;
                } }
                object IEnumerator.Current => throw new NotImplementedException();

                #endregion

                List byteNodeList;
                ListNode* currentListNodePtr;
                bool isEnd;

                public ref ByteNode CurrentRef { get {
                    if (currentListNodePtr == null) 
                        throw new Exception("현재 노드가 없습니다.");
                    
                    return ref currentListNodePtr->byteNode;
                } }

                #endregion

                // 생성자
                public Enumerator(List byteNodeList)
                {
                    this.byteNodeList = byteNodeList;
                    this.currentListNodePtr = null;
                    this.isEnd = (byteNodeList.headNodePtr == null);
                }


                #region Method

                #region 명시적 인터페이스 구현

                void IEnumerator.Reset() => throw new NotImplementedException();
                void IDisposable.Dispose() => throw new NotImplementedException();

                #endregion

                public bool MoveNext()
                {
                    if (isEnd) return false;

                    if (currentListNodePtr == null) {
                        currentListNodePtr = byteNodeList.headNodePtr;
                        return true;
                    }
                    currentListNodePtr = currentListNodePtr->nextNodePtr;

                    return currentListNodePtr != null;
                }

                #endregion
            }
            
            #endregion


            #region Instance

            #region Field & Property

            public ListNode* headNodePtr;
            public ref ByteNode HeadByteNodeRef => ref headNodePtr->byteNode;

            int count;
            public int Count => count;

            public bool IsEmpty => (headNodePtr == null);

            #endregion


            // Constructor
            public List()
            {
                headNodePtr = null;
                count = 0;
            }


            #region Method

            #region 명시적 인터페이스 구현

            IEnumerator<ByteNode> IEnumerable<ByteNode>.GetEnumerator()
                => new Enumerator(this);

            IEnumerator IEnumerable.GetEnumerator()
                => new Enumerator(this);

            #endregion


            /// <summary>
            /// 노드를 앞부터 탐색해 값이 존재한다면 바이트 노드의 포인터를 반환하고 <br/>
            /// 없다면 리스트 노드와 바이트 노드를 생성하고 반환합니다.
            /// </summary>
            /// <param name="keyByte"></param>
            /// <returns>"keyByte"를 가지는 바이트 노드 (notnull)</returns>
            public ref ByteNode GetOrCreateByteNodeRef(byte keyByte)
            {
                ListNode* closestNodePtr = FindClosestNodePtr(keyByte);

                // 찾는 바이트보다 작거나 같은 바이트를 가지는 노드가 없음 => 리스트의 헤드에 노드 생성 후 반환
                if (closestNodePtr == null)
                {
                    // 리스트의 헤드에 노드 생성
                    headNodePtr = ListNode.Create(keyByte, null, headNodePtr);
                    count++;

                    return ref headNodePtr->byteNode;
                }
                
                // 찾은 노드 바이트가 찾는 바이트와 같음 => 찾은 노드 반환
                if (closestNodePtr->byteNode.keyByte == keyByte)
                {
                    return ref closestNodePtr->byteNode;
                }
                // 찾은 노드 바이트가 찾는 바이트보다 작음 => 찾은 노드 다음에 노드 생성
                else
                {
                    // 찾은 노드 다음에 노드 생성
                    closestNodePtr->nextNodePtr = ListNode.Create(keyByte, closestNodePtr, closestNodePtr->nextNodePtr);
                    count++;

                    return ref headNodePtr->byteNode;
                }
            }
            
            /// <summary>
            /// "keyByte"를 가지는 바이트 노드의 포인터를 반환함
            /// </summary>
            /// <param name="keyByte"></param>
            /// <returns></returns>
            public ref ByteNode GetByteNodeRef(byte keyByte)
            {
                ListNode* listNodePtr = FindListNodePtr(keyByte);
                if (listNodePtr == null)
                    throw new KeyNotFoundException($"'{keyByte}'를 가지는 바이트 노드가 없습니다.");

                return ref listNodePtr->byteNode;
            }
            
            /// <summary>
            /// "keyByte"를 가지는 바이트 노드의 리스트 노드를 반환함  <br/>
            /// </summary>
            /// <param name="keyByte"></param>
            /// <returns></returns>
            ListNode* FindListNodePtr(byte keyByte)
            {
                ListNode* closestNodePtr = FindClosestNodePtr(keyByte);

                // 찾는 바이트보다 작은 바이트를 가지는 노드가 없음 -> 헤드 노드(nullable) 반환
                if (closestNodePtr == null)
                    return headNodePtr;
                
                // 찾은 노드 바이트가 찾는 바이트와 같음 -> 찾은 노드 반환
                if (closestNodePtr->byteNode.keyByte == keyByte)
                {
                    return closestNodePtr;
                }
                // 노드 바이트가 찾는 바이트보다 작음 -> null 반환
                else
                {
                    return null;
                }
            }
            
            /// <summary>
            /// "keyByte" 보다 작거나 같은 바이트 노드중 가장 큰 노드의 리스트 노드 을 반환함
            /// </summary>
            /// <param name="keyByte">찾을 바이트</param>
            /// <returns>null: 헤드 노드가 없거나 헤드 노드 바이트 노드의 바이트가 "keyByte" 보다 큼</returns>
            ListNode* FindClosestNodePtr(byte keyByte)
            {
                // 헤드 노드가 없음 -> null 반환
                if (headNodePtr == null)
                    return null;

                // 헤드 노드 바이트가 찾는 바이트보다 큼 -> null 반환
                if (headNodePtr->byteNode.keyByte > keyByte)
                    return null;


                ListNode* findNodePtr = headNodePtr;
                ListNode* prevNodePtr = null;

                while (findNodePtr != null)
                {
                    byte findKeyByte = findNodePtr->byteNode.keyByte;

                    // 찾는 바이트와 일치함 -> 현재 노드 반환
                    if (findKeyByte == keyByte)
                    {
                        return findNodePtr;
                    }
                    
                    // 찾는 바이트보다 작음 -> 다음 노드로 이동
                    if (findKeyByte < keyByte)
                    {
                        prevNodePtr = findNodePtr;
                        findNodePtr = findNodePtr->nextNodePtr;
                        continue;
                    }
                    // 찾는 바이트를 초과함 -> 이전 노드 반환
                    else
                    {
                        return prevNodePtr;
                    }
                }

                // 찾는 바이트보다 작은 노드가 존재하지 않음 -> 마지막 노드 반환
                return prevNodePtr;
            }
            
            public Enumerator GetEnumerator()
                => new Enumerator(this);
            
            public void Dispose()
            {
                ListNode* findNodePtr = headNodePtr;
                ListNode* nextNodePtr = null;

                while (findNodePtr != null)
                {
                    // 찾은 노드의 바이트 노드 Dispose
                    findNodePtr->Dispose();

                    // 다음 노드 포인터 임시 저장
                    nextNodePtr = findNodePtr->nextNodePtr;
                    
                    // 찾은 노드 메모리 해제
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
        internal unsafe struct ListNode : IDisposable
        {
            #region Static

            public static ListNode* Create(byte keyByte, ListNode* prevListNodePtr, ListNode* nextListNodePtr)
            {
                // 리스트 노드 생성
                ListNode* newListNodePtr = (ListNode*)Marshal.AllocHGlobal(sizeof(ListNode));

                // prev -> new
                if (prevListNodePtr != null)
                {
                    prevListNodePtr->nextNodePtr = newListNodePtr;
                }

                *newListNodePtr = new ListNode(
                    new ByteNode(keyByte),
                    nextListNodePtr
                );

                // new -> find
                return newListNodePtr;
            }

            #endregion


            #region Instance

            public ByteNode byteNode;
            public ListNode* nextNodePtr;

            ListNode(ByteNode byteNode, ListNode* next)
            {
                this.byteNode = byteNode;
                this.nextNodePtr = next;
            }

            public void Dispose()
            {
                byteNode.Dispose();
            }

            #endregion
        }

        #endregion


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
        public readonly ByteNode.List childList;
        
        ByteNode* parentPtr;
        public ref ByteNode ParentNodeRef 
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

        public void Dispose()
        {
            childList.Dispose();
        }

        #endregion

        #endregion
    }
    
    struct ReadOnlyTreeBuildData
    {
        public int nodeCount;
        public int valueCount;
        public int maxNodeDepth;
        public int maxBranchDepth;

        public ReadOnlyTreeBuildData()
        {
            this.nodeCount = 0;
            this.valueCount = 0;
            this.maxNodeDepth = 0;
            this.maxBranchDepth = 0;
        }
        public ReadOnlyTreeBuildData(int nodeCount = 0, int valueCount = 0, int maxNodeDepth = 0, int maxBranchDepth = 0)
        {
            this.nodeCount = nodeCount;
            this.valueCount = valueCount;
            this.maxNodeDepth = maxNodeDepth;
            this.maxBranchDepth = maxBranchDepth;
        }
    }
    

    #region Public

    public struct ValueEnumerator : IEnumerator<TValue>, IDisposable
    {
        NodeEnumerator nodeIterator;

        ByteNode valueNode;
        TValue IEnumerator<TValue>.Current { get { 
            if (valueNode.HasValue == false)
                throw new Exception("노드가 없거나 노드에 값이 없습니다.");

            return valueNode.Value;
        } }
        
        public object? Current { get { 
            if (valueNode.HasValue == false)
                throw new Exception("노드가 없거나 노드에 값이 없습니다.");

            return valueNode.Value;
        } }

        public ValueEnumerator(TempByteSpanTree<TValue> tree)
        {
            this.nodeIterator = tree.GetNodeIterator();
        }
        
        public bool MoveNext()
        {
            while (nodeIterator.MoveNext())
            {
                ref ByteNode currentNode = ref nodeIterator.Current;
                // 찾은 노드가 값이 있음 -> true 반환
                if (currentNode.(out TValue value))
                {
                    valueNode = currentNode;
                    return true;
                }
                {
                    valueNode = currentNode;
                    return true;
                }
            }
            return false;
        }
        public bool MoveNext([NotNullWhen(true)] out TValue? current)
        {
            ByteNode? currentNode;
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

    unsafe internal ref struct NodeEnumerator
    {
        TempByteSpanTree<TValue> tree;

        UnmanagedStack<int> nodeFindIndexTrace;

        public ByteNode.ListNode* currentListNodePtr;
        public ByteNode* CurrentPtr => &currentListNodePtr->byteNode;
        public ref ByteNode CurrentRef => ref currentListNodePtr->byteNode;

        bool isEnd;


        // 생성자
        public NodeEnumerator(TempByteSpanTree<TValue> tree)
        {
            this.tree = tree;
            this.nodeFindIndexTrace = new UnmanagedStack<int>(tree.maxBranchDepth + 1);

            this.CurrentPtr = null;

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
            if (currentListNodePtr == null) {
                currentListNodePtr = tree.byteNodeList.headNodePtr;
                nodeFindIndexTrace.Push(-1);
                return true;
            }

            // 현재 노드가 브랜치 노드임 -> 찾을 노드가 남았으면 다음 노드/true 반환
            if (CurrentPtr->childList.Count > 0)
            {
                ref int nodeFindIndex = ref nodeFindIndexTrace.Peek();

                // 아직 찾을 노드가 남았음 -> 다음 노드/true 반환
                if (nodeFindIndex + 1 < CurrentPtr->childList.Count)
                {
                    // 인덱스 이동
                    nodeFindIndex++;

                    // 다음 노드로 이동하고 반환
                    CurrentPtr = CurrentPtr.childNodeArray[nodeFindIndex];
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

                ref int findNodeIndex = ref nodeFindIndexTrace.Peek();

                // 브랜치에 찾을 노드가 남아있음
                if (findNodeIndex + 1 < lastBranch.Length)
                {
                    // 인덱스 이동
                    findNodeIndex++;

                    // 다음 노드로 이동하고 반환
                    CurrentPtr = lastBranch[findNodeIndex];
                    nodeFindIndexTrace.Push(-1);
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
            ReadOnlyByteSpanTree<TValue>.CreateEmptyTreeRootNode(),
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
            ReadOnlyByteSpanTree<TValue>.CreateValueTreeRootNode(
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
        ReadOnlyTreeBuildData buildData = new (
            nodeCount: 1
        );

        ReadOnlyByteSpanTree<TValue>.Node[] childNodeArray 
            = CreateNodeArray(
                1,
                branchDepth,
                byteNodeList, 
                ref buildData
            );

        return new ReadOnlyByteSpanTree<TValue>(
            ReadOnlyByteSpanTree<TValue>.CreateBranchTreeRootNode(
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
        ReadOnlyTreeBuildData buildData = new (
            nodeCount: 1
        );

        ReadOnlyByteSpanTree<TValue>.Node[] childNodeArray 
            = CreateNodeArray(
                2,
                branchDepth,
                byteNodeList, 
                ref buildData
            );

        return new ReadOnlyByteSpanTree<TValue>(
            ReadOnlyByteSpanTree<TValue>.CreateValueBranchTreeRootNode(
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
    static ReadOnlyByteSpanTree<TValue>.Node CreateValueNode(byte[] keySlice, TValue value, int nodeDepth, int branchDepth, ref ReadOnlyTreeBuildData buildData)
    {
        buildData.maxNodeDepth = Math.Max(buildData.maxNodeDepth, nodeDepth);
        buildData.maxBranchDepth = Math.Max(buildData.maxBranchDepth, branchDepth);

        return ReadOnlyByteSpanTree<TValue>.CreateValueNode(
            buildData.nodeCount++, 
            keySlice, 
            new ReadOnlyByteSpanTree<TValue>.ValueInfo(value, buildData.valueCount++),
            branchDepth
        );
    }

    /// <summary>
    /// 브랜치 노드 생성
    /// </summary>
    /// <param name="keyByte"></param>
    /// <param name="childNodeInfo"></param>
    static ReadOnlyByteSpanTree<TValue>.Node CreateBranchNode(byte[] keySlice, ReadOnlyByteSpanTree<TValue>.Node[] childNodeArray, int branchDepth, ref ReadOnlyTreeBuildData buildData)
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
    static ReadOnlyByteSpanTree<TValue>.Node CreateValueBranchNode(byte[] keySlice, TValue value, ReadOnlyByteSpanTree<TValue>.Node[] childNodeArray, int branchDepth, ref ReadOnlyTreeBuildData buildData)
    {
        return ReadOnlyByteSpanTree<TValue>.CreateValueBranchNode(
            buildData.nodeCount++, 
            keySlice, 
            new ReadOnlyByteSpanTree<TValue>.ValueInfo(value, buildData.valueCount++), 
            childNodeArray,
            branchDepth
        );
    }

    #endregion

    #region 재귀적 노드 생성

    static ReadOnlyByteSpanTree<TValue>.Node CreateNode(byte[] keySlice, ref ByteNode byteNode, int nodeDepth, int branchDepth, ref ReadOnlyTreeBuildData buildData)
    {
        // 자식이 없음 -> 값 노드
        if (byteNode.childListPtr.IsEmpty)
        {
            if (byteNode.TryGetValue(out TValue? value))
            {
                return CreateValueNode(
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
            ReadOnlyByteSpanTree<TValue>.Node[] childNodeArray = CreateNodeArray(
                nodeDepth, 
                branchDepth,
                byteNode.childListPtr, 
                ref buildData
            );
            
            // 값 브랜치 노드
            if (byteNode.TryGetValue(out TValue? value))
            {
                return CreateValueBranchNode(
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
                return CreateBranchNode(
                    keySlice,
                    childNodeArray,
                    branchDepth,
                    ref buildData
                );
            }
        }
    }
    
    static ReadOnlyByteSpanTree<TValue>.Node[] CreateNodeArray(int parentNodeDepth, int parentBranchDepth, ByteNode.List byteNodeList, ref ReadOnlyTreeBuildData buildData)
    {
        int byteNodeListCount = byteNodeList.Count;
        
        if (byteNodeListCount == 0)
            throw new ArgumentException("byteNodeList가 비었습니다.");

        ByteNode.List.Enumerator byteNodeIterator = byteNodeList.GetEnumerator();

        ReadOnlyByteSpanTree<TValue>.Node[] childArray = new ReadOnlyByteSpanTree<TValue>.Node[byteNodeListCount];

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

    #region 다음 값 또는 브랜치노드 찾기

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
            if (findByteNode.childListPtr.Count == 1)
            {
                moveCount++;
                findByteNode = findByteNode.childListPtr.HeadByteNode;
                continue;
            }

            // 자식 노드가 2개 이상 -> 반환
            if (findByteNode.childListPtr.Count > 1)
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
            findByteNode = ref findByteNode.childListPtr.HeadByteNode;

            keySlice[i] = findByteNode.keyByte;
        }

        return ref byteNode;
    }
    
    #endregion

    #endregion


    #region Instance

    #region 필드

    ByteNode.List byteNodeList;

    #endregion


    // 생성자
    public TempByteSpanTree()
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

        ref ByteNode findNode = ref byteNodeList.GetOrCreateByteNodeRef(keyByte);

        int findByteIndex = 0;
        int keyLength = key.Length;

        while (findByteIndex < keyLength)
        {
            // 키 바이트
            keyByte = key[findByteIndex];

            // 다음으로 이동
            findNode = ref findNode.childListPtr.GetOrCreateByteNode(keyByte);
            findByteIndex++;
        }

        // 찾은 노드에 값을 설정
        findNode.SetValue(value);
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
    
    public ValueEnumerator GetValueIterator()
        => new ValueIterator(this);
    public TValue[] ToArray()
    {
        TValue[] valueArray = new TValue[valueCount];

        using (ValueEnumerator valueIterator = GetValueIterator())
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


    public ReadOnlyByteSpanTree<TValue> BuildToReadOnlyByteSpanTree()
    {
        // 바이트 노드가 없음 -> 빈 트리 반환
        if (byteNodeList.IsEmpty)
        {
            return CreateEmptyTree();
        }
        // 바이트 노드가 있음
        else
        {
            ref ByteNode startNode = ref byteNodeList.HeadByteNode;

            byte[] keySlice;
            ref ByteNode nextByteNode = ref GetNextValueOrBranchNode(ref startNode, out keySlice);

            // 첫번째 분기에 값이 있음
            if (nextByteNode.TryGetValue(out TValue? value))
            {
                if (nextByteNode.childListPtr.IsEmpty)
                {
                    // 단 하나의 값을 가지는 트리 생성
                    return CreateValueTree(
                        keySlice,
                        value
                    );
                }
                else
                {
                    // 값을 가지며 동시에 하나 이상의 자식을 갖는 트리 생성
                    return CreateValueBranchTree(
                        keySlice,
                        value, 
                        nextByteNode.childListPtr,
                        1
                    );
                }
            }
            else
            {
                if (nextByteNode.childListPtr.IsEmpty)
                {
                    throw new InvalidOperationException("잘못된 바이트 노드가 존재합니다.");
                }
                else
                {
                    // 둘 이상의 자식을 갖는 트리 생성
                    return CreateBranchTree(
                        keySlice,
                        nextByteNode.childListPtr,
                        1
                    );
                }
            }
        }
    }
    
    public ReadOnlyByteSpanTree<TValue> BuildToReadOnlyByteSpanTreeAndDispose()
    {
        // 트리 빌드
        ReadOnlyByteSpanTree<TValue> tree = BuildToReadOnlyByteSpanTree();

        // Dispose
        Dispose();
        
        // 빌드한 트리 반환
        return tree;
    }

    public void Dispose()
    {
        // 노드 리스트를 Dispose
        byteNodeList.Dispose();
    }

    #endregion

    #endregion
}