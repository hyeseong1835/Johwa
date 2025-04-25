namespace Johwa.Common.Collection;

public ref struct TempByteSpanTree<TValue>
{
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

    ReadOnlyByteSpanTree<TValue> originalTree;
    Span<TValue> valueSpan;

    public TempByteSpanTree(ReadOnlyByteSpanTree<TValue> originalTree, Span<TValue> valueBuffer)
    {
        this.originalTree = originalTree;
        this.valueSpan = valueBuffer;
    }
}