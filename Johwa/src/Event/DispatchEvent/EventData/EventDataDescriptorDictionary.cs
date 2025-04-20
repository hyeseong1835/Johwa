using System.Buffers;

namespace Johwa.Event.Data;

public class EventDataDescriptorDictionary
{
    #region Object

    unsafe public struct TreeNodeData
    {
        public byte keyStartByte;
        public int keyLength;
        public fixed int nameIndexArray[64];

        public TreeNodeData(byte keyStartByte, int keyLength)
        {
            this.keyStartByte = keyStartByte;
            this.keyLength = keyLength;
        }

        public void AddKeyLength()
        {
            keyLength++;
        }
    }
    public class TreeNode
    {
        public byte startByte;
        public int length;
        public TreeNode[] children;

        public TreeNode(byte startByte, int length, TreeNode[] children)
        {
            this.startByte = startByte;
            this.length = length;
            this.children = children;
        }
    }

    #endregion


    #region Static


    #endregion


    #region Instance

    // 필드

    public TreeNode treeRoot;

    // 생성자
    public EventDataDescriptorDictionary(EventDataContainerMetadata containerMetadata)
    {
        // 이름 배열 로드
        IEnumerable<EventDataDescriptor> descriptorEnumerable = containerMetadata.GetEventDataDescriptorEnumerable();
        int descriptorCount = descriptorEnumerable.Count();

        if (descriptorCount == 0)
            throw new ArgumentException("이벤트 데이터 설명자가 없습니다.");

        string[] nameArray = ArrayPool<string>.Shared.Rent(descriptorCount);

        int descriptorIndex = 0;
        foreach (EventDataDescriptor descriptor in descriptorEnumerable)
        {
            nameArray[descriptorIndex++] = descriptor.Name;
        }

        treeRoot = GetTreeNode(nameArray, 0, descriptorCount);

        /// <summary>
        /// 트리 노드 생성
        /// </summary>
        static TreeNode[]? GetTreeNodes(string[] nameArray, int byteIndex, int maxNodeCount)
        {
            Span<TreeNodeData> nodeDataSpan = stackalloc TreeNodeData[maxNodeCount];
            int nodeDataCount = 0;

            bool isExist = false;

            for (int nameIndex = 0; nameIndex < nameArray.Length; nameIndex++)
            {
                string name = nameArray[nameIndex];
                
                // 길이 초과시 continue
                if (byteIndex >= name.Length) 
                    continue;

                byte c = (byte)name[byteIndex];

                // 값이 있는 첫 번째 값이면
                if (isExist == false) 
                {
                    isExist = true;
                }


                bool isExistCharacter = false;                
                for (int nodeDataIndex = 0; nodeDataIndex < nodeDataCount; nodeDataIndex++)
                {
                    ref TreeNodeData nodeData = ref nodeDataSpan[nodeDataIndex];

                    if (nodeData.keyStartByte == c)
                    {
                        
                        break;
                    }
                }
                if (isExistCharacter == false)
                {
                    
                }
            }

            // 모두 값이 없으면 null 리턴
            if (isExist == false)
            {
                return null;
            }

            for (int nodeDataIndex = 0; nodeDataIndex < nodeDataCount; nodeDataIndex++)
            {
                ref TreeNodeData nodeData = ref nodeDataSpan[nodeDataIndex];

                for (int nextByteIndex = byteIndex + 1; ; nextByteIndex++)
                {
                    TreeNode[]? children = GetTreeNodes(nameArray, nextByteIndex, nodeData.keyLength);
                    if (children != null) {
                        break;
                    }

                    nodeData.AddKeyLength();
                }
            }

            static void 
        }
    }

    #endregion
}