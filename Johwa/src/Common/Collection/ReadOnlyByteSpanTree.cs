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

            public TValue? value;
            public bool hasValue;


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
            
            /// <summary>
            /// 값이 있는 노드 생성
            /// </summary>
            /// <param name="keyByte"></param>
            /// <param name="value"></param>
            public ByteNode(byte keyByte, TValue value)
            {
                this.keyByte = keyByte;
                this.childList = new ByteNodeList();

                this.value = value;
                this.hasValue = true;
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
            findNode.value = value;
        }

        public ReadOnlyByteSpanTree<TValue> Build()
        {
            // 노드가 비었음 -> 빈 트리 반환
            if (nodeList.IsEmpty)
            {
                ChildNodeInfo rootChildNodeInfo = new ChildNodeInfo(
                    [ ], 
                    default
                );

                return new ReadOnlyByteSpanTree<TValue>(rootChildNodeInfo);
            }

            ByteNodeList byteNodeList = nodeList;
            int byteNodeListDepth = 0;

            while (true)
            {
                // 노드 리스트의 노드 꺼내기
                ref ByteNode byteNode = ref byteNodeList.HeadByteNode;

                // 노드가 값을 가지고 있음 -> 노드 생성
                TValue? value;
                if (byteNode.TryGetValue(out value))
                {
                    // 노드의 자식 노드 리스트가 비어있음 -> 값 노드 생성
                    if (byteNode.childList.IsEmpty)
                    {
                        // 값 노드 생성
                        Node node = new Node(
                            byteNode.keyByte,
                            value
                        );
                        ChildNodeInfo rootChildNodeInfo = new ChildNodeInfo(
                            [ node ],
                            byteNodeListDepth
                        );
                        return new ReadOnlyByteSpanTree<TValue>(rootChildNodeInfo);
                    }
                    else
                    {
                        // 자식 노드 리스트가 비어있지 않음 -> 값 브랜치 노드 생성
                        ChildNodeInfo childNodeInfo = new ChildNodeInfo(
                            CreateChildNodeArray(
                                byteNodeList, 
                                byteNodeListDepth
                            ), 
                            byteNodeListDepth
                        );
                        Node node = new Node(
                            byteNode.keyByte,
                            value,
                            childNodeInfo
                        );
                        ChildNodeInfo rootChildNodeInfo = new ChildNodeInfo(
                            [ node ],
                            byteNodeListDepth
                        );

                        return new ReadOnlyByteSpanTree<TValue>(rootChildNodeInfo);
                    }
                }

                // 자식 노드가 1개임 -> 다음 depth로 이동
                if (byteNodeList.Count == 1)
                {
                    byteNodeList = byteNodeList.HeadByteNode.childList;
                    byteNodeListDepth++;
                    continue;
                }

                // 자식 노드가 2개 이상 -> 브랜치 노드 생성
                if (byteNodeList.Count > 1)
                {
                    Node[] childArray = CreateChildNodeArray(
                        byteNodeList,
                        byteNodeListDepth
                    );
                    ChildNodeInfo rootChildNodeInfo = new ChildNodeInfo(
                        childArray,
                        byteNodeListDepth
                    );

                    return new ReadOnlyByteSpanTree<TValue>(rootChildNodeInfo);
                }

                // 노드 리스트가 비어있음 -> 예외 발생
                if (byteNodeList.IsEmpty)
                {
                    throw new ArgumentException("노드 리스트가 비어 있습니다.");
                }
            }
        }
        
        static Node CreateNode(ByteNodeList byteNodeList, int byteNodeListDepth)
        {
            while (true)
            {
                // 노드 리스트의 노드 꺼내기
                ref ByteNode byteNode = ref byteNodeList.HeadByteNode;

                // 노드가 값을 가지고 있음 -> 노드 생성
                TValue? value;
                if (byteNode.TryGetValue(out value))
                {
                    // 노드의 자식 노드 리스트가 비어있음 -> 값 노드 생성
                    if (byteNode.childList.IsEmpty)
                    {
                        // 값 노드 생성
                        return new Node(
                            byteNode.keyByte,
                            value
                        );
                    }
                    else
                    {
                        // 자식 노드 리스트가 비어있지 않음 -> 값 브랜치 노드 생성
                        return new Node(
                            byteNode.keyByte,
                            value,
                            new ChildNodeInfo(
                                CreateChildNodeArray(
                                    byteNodeList, 
                                    byteNodeListDepth
                                ), 
                                byteNodeListDepth
                            )
                        );
                    }
                }

                // 자식 노드가 1개임 -> 다음 depth로 이동
                if (byteNodeList.Count == 1)
                {
                    byteNodeList = byteNodeList.HeadByteNode.childList;
                    byteNodeListDepth++;
                    continue;
                }

                // 자식 노드가 2개 이상 -> 브랜치 노드 생성
                if (byteNodeList.Count > 1)
                {
                    Node[] childArray = CreateChildNodeArray(
                        byteNodeList,
                        byteNodeListDepth
                    );

                    return new Node(
                        byteNode.keyByte,
                        new ChildNodeInfo(
                            childArray,
                            byteNodeListDepth
                        )
                    );
                }

                // 노드 리스트가 비어있음 -> 예외 발생
                if (byteNodeList.IsEmpty)
                {
                    throw new ArgumentException("노드 리스트가 비어 있습니다.");
                }
            }
        }
        
        static Node[] CreateChildNodeArray(ByteNodeList byteNodeList, int byteNodeListDepth)
        {
            Node[] childArray = new Node[byteNodeList.Count];

            ByteNodeList.Iterator byteNodeIterator = byteNodeList.GetIterator();
            ref ByteNode byteNode = ref byteNodeIterator.Current;

            for (int i = 0; i < byteNodeList.Count; i++)
            {
                // 자식 리스트를 빌드
                childArray[i] = CreateNode(
                    byteNode.childList, 
                    byteNodeListDepth
                );

                // 다음 노드로 이동
                byteNodeIterator.MoveNext();
                byteNode = ref byteNodeIterator.Current;
            }

            return childArray;
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

    public class Node
    {
        public readonly byte keyByte;

        public readonly TValue? value;
        public readonly bool hasValue;

        public readonly ChildNodeInfo? childNodeInfo;


        #region Constructor

        /// <summary>
        /// 값 노드 생성
        /// </summary>
        /// <param name="value"></param>
        public Node(byte keyByte, TValue value)
        {
            this.keyByte = keyByte;

            this.value = value;
            this.hasValue = true;
            
            this.childNodeInfo = null;
        }

        /// <summary>
        /// 브랜치 노드 생성
        /// </summary>
        /// <param name="keyByte"></param>
        /// <param name="childNodeInfo"></param>
        public Node(byte keyByte, ChildNodeInfo childNodeInfo)
        {
            this.keyByte = keyByte;

            this.value = default;
            this.hasValue = false;

            this.childNodeInfo = childNodeInfo;
        }

        /// <summary>
        /// 값 브랜치 노드 생성
        /// </summary>
        /// <param name="keyByte"></param>
        /// <param name="childNodeInfo"></param>
        /// <param name="value"></param>
        public Node(byte keyByte,  TValue value, ChildNodeInfo childNodeInfo)
        {
            this.keyByte = keyByte;

            this.value = value;
            this.hasValue = true;

            this.childNodeInfo = childNodeInfo;
        }

        #endregion
    }
    
    public class ChildNodeInfo
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

    #endregion

    #region Instance

    readonly ChildNodeInfo rootChildNodeInfo;

    ReadOnlyByteSpanTree(ChildNodeInfo rootChildNodeInfo)
    {
        this.rootChildNodeInfo = rootChildNodeInfo;
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

    #endregion
}