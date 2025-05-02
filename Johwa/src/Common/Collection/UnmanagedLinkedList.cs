using System.Collections;
using System.Runtime.InteropServices;

namespace Johwa.Common.Collection;

/// <summary>
/// 관리되지 않는 메모리에서 싱글 링크드 리스트를 구현합니다. <br/>
/// <br/>
/// 더 이상 사용하지 않을 때 Dispose()를 호출해야합니다.
/// </summary>
/// <typeparam name="T"></typeparam>
unsafe public struct UnmanagedLinkedList<T> : IEnumerable<T>
    where T : unmanaged
{
    #region Object

    struct Node
    {
        #region Static

        public static Node* Create(T value, Node* nextNodePtr)
        {
            // 관리되지 않는 힙에 메모리 할당
            Node* ptr = (Node*)Marshal.AllocHGlobal(sizeof(Node));
            
            // 노드 생성
            *ptr = new Node(value, nextNodePtr);

            return ptr;
        }

        #endregion


        #region Instance
    
        public T value;
        public Node* nextNodePtr;

        // 생성자
        Node(T value, Node* nextNodePtr)
        {
            this.value = value;
            this.nextNodePtr = nextNodePtr;
        }

        #endregion
    }

    public struct Enumerator : IEnumerator<T>
    {
        #region Field & Property

        UnmanagedLinkedList<T> list;

        Node* currentNodePtr;
        public T Current => currentNodePtr->value;
        public ref T CurrentRef => ref currentNodePtr->value;
        object IEnumerator.Current => Current;

        bool isEnd;

        #endregion


        #region Constructor

        public Enumerator(UnmanagedLinkedList<T> list)
        {
            this.list = list;
            this.currentNodePtr = null;
            this.isEnd = false;
        }
        public Enumerator(Enumerator enumerator)
        {
            this.list = enumerator.list;
            this.currentNodePtr = enumerator.currentNodePtr;
            this.isEnd = enumerator.isEnd;
        }

        #endregion


        #region Method

        #region 명시적 인터페이스 구현

        void IDisposable.Dispose() 
            => throw new NotImplementedException();

        #endregion

        public bool MoveNext()
        {
            if (currentNodePtr == null)
                return false;

            currentNodePtr = currentNodePtr->nextNodePtr;
            return currentNodePtr != null;
        }
       
        public Enumerator Copy()
            => new Enumerator(this);

        public void Reset()
        {
            currentNodePtr = null;
            isEnd = false;
        }

        #endregion
    }
    
    public ref struct RefEnumerator
    {
        #region Field & Property

        ref UnmanagedLinkedList<T> listRef;

        Node* currentNodePtr;
        public T Current => currentNodePtr->value;
        public ref T CurrentRef => ref currentNodePtr->value;
        public T* CurrentPtr => &(currentNodePtr->value);

        bool isEnd;

        #endregion


        #region Constructor

        public RefEnumerator(ref UnmanagedLinkedList<T> list)
        {
            this.listRef = ref list;
            this.currentNodePtr = null;
            this.isEnd = false;
        }
        public RefEnumerator(RefEnumerator enumerator)
        {
            this.listRef = ref enumerator.listRef;
            this.currentNodePtr = enumerator.currentNodePtr;
            this.isEnd = enumerator.isEnd;
        }

        #endregion


        #region Method

        public bool MoveNext()
        {
            if (currentNodePtr == null)
                return false;

            currentNodePtr = currentNodePtr->nextNodePtr;
            return currentNodePtr != null;
        }
        public void InsertNext(T value)
        {
            // 새 노드 생성 (new -> next)
            Node* newNodePtr = Node.Create(value, currentNodePtr->nextNodePtr);

            // (cur -> new)
            currentNodePtr->nextNodePtr = newNodePtr;
        }
        public void InsertPrev(T value)
        {
            // 새 노드 생성 (prev -> new)
            Node* newNodePtr = Node.Create(value, currentNodePtr);

            // (new -> cur)
            currentNodePtr = newNodePtr;
        }
        public RefEnumerator Copy()
            => new RefEnumerator(this);

        public void Reset()
        {
            currentNodePtr = null;
            isEnd = false;
        }

        #endregion
    }
    
    #endregion


    #region Instance

    #region Field & Property

    Node* headNodePtr;
    public T HeadValue => (headNodePtr->value);
    public ref T HeadValueRef => ref (headNodePtr->value);
    public T* HeadValuePtr => &(headNodePtr->value);

    Node* tailNodePtr;
    public T TailValue => tailNodePtr->value;
    public ref T TailValueRef => ref tailNodePtr->value;
    public T* TailValuePtr => &(tailNodePtr->value);

    int count;
    public int Count => count;
    public bool IsEmpty => count == 0;

    #endregion


    #region Constructor

    public UnmanagedLinkedList()
    {
        this.headNodePtr = null;
        this.count = 0;
    }

    #endregion


    #region Method

    #region 명시적 인터페이스 구현

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
        => GetEnumerator();

    #endregion

    public ref T AddFirst(T value)
    {
        headNodePtr = Node.Create(value, headNodePtr);

        if (count == 0) 
        {
            tailNodePtr = headNodePtr;
        }
        
        count++;

        return ref headNodePtr->value;
    }
    public ref T AddLast(T value)
    {
        Node* newNodePtr = Node.Create(value, null);

        if (count == 0) 
        {
            headNodePtr = newNodePtr;
            tailNodePtr = newNodePtr;
        }
        else
        {
            tailNodePtr->nextNodePtr = newNodePtr;
            tailNodePtr = newNodePtr;
        }

        count++;

        return ref tailNodePtr->value;
    }
    
    public void RemoveFirst()
    {
        if (headNodePtr == null)
            throw new InvalidOperationException("리스트가 비어 있습니다.");

        Node* secondNodePtr = headNodePtr->nextNodePtr;

        Marshal.FreeHGlobal((IntPtr)headNodePtr);

        headNodePtr = secondNodePtr;
    }
    
    public Enumerator GetEnumerator()
        => new Enumerator(this);

    public void Dispose()
    {
        Node* currentNodePtr = headNodePtr;
        Node* nextNodePtr;

        for (int i = 0; i < count; i++)
        {
            // 임시로 다음 노드 포인터를 저장
            nextNodePtr = currentNodePtr->nextNodePtr;

            // 현재 노드 포인터를 해제
            Marshal.FreeHGlobal((IntPtr)currentNodePtr);
            
            // 현재 노드 포인터를 다음 노드 포인터로 이동
            currentNodePtr = nextNodePtr;
        }
        headNodePtr = null;
    }
    
    #endregion

    #endregion
}