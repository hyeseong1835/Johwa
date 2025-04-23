using System.Buffers;
using Johwa.Common.Collection;
using Johwa.Common.Debug;
using Johwa.Common.Extension.Johwa.Common.Collection;

namespace Johwa.Event.Data;

public class EventDataDescriptorTree
{
    #region Object

    struct TreeNodeFindData
    {
        public ref struct Set
        {
            // 필드 & 프로퍼티
            public Span<TreeNodeFindData> branchFindDataSpan;
            public readonly int findNameIndex;

            public int ByteTypeCount => byteTypeCount;
            public ref TreeNodeFindData MaxNameCountData => ref maxNameCountData;
            public int MaxNameCountDataIndex => maxNameNodeFindDataIndex;
            public ref TreeNodeFindData SecondNameCountData => ref secondNameCountData;

            int byteTypeCount;
            ref TreeNodeFindData maxNameCountData;
            int maxNameNodeFindDataIndex;
            ref TreeNodeFindData secondNameCountData;


            // 생성자
            public Set(Span<TreeNodeFindData> branchFindDataSpan, int findNameIndex)
            {
                this.branchFindDataSpan = branchFindDataSpan;
                this.byteTypeCount = 0;
                this.findNameIndex = findNameIndex;
            }
            

            #region 메서드

            #region 공개 메서드
            
            public ref TreeNodeFindData this[int index] => ref Get(index);
            public ref TreeNodeFindData Get(int index)
            {
                if (index < 0 || index >= byteTypeCount)
                    throw new ArgumentOutOfRangeException("인덱스가 범위를 벗어났습니다.");

                return ref branchFindDataSpan[index];
            }
            
            public void AddValue(EventDataDescriptor descriptor)
            {
                if (byteTypeCount >= branchFindDataSpan.Length)
                    throw new ArgumentOutOfRangeException("브랜치 데이터가 너무 많습니다.");

                if (descriptor.Name.Length == 0) 
                    throw new ArgumentException("이름 길이가 0입니다.");

                if (findNameIndex >= descriptor.Name.Length) 
                    throw new ArgumentOutOfRangeException("이름 길이를 초과했습니다.");

                if (byteTypeCount == 0)
                {
                    AddData(new TreeNodeFindData(
                        (byte)descriptor.Name[findNameIndex], 
                        1
                    ));
                    return;
                }

                bool isFound;
                int foundedIndex;

                ref TreeNodeFindData branchFindData = ref FindData(
                    (byte)descriptor.Name[findNameIndex], 
                    out isFound,
                    out foundedIndex
                );

                if (isFound)
                {
                    branchFindData.nameCount++;
                }
                else
                {
                    branchFindData = AddData(new TreeNodeFindData(
                        (byte)descriptor.Name[0], 
                        1
                    ));
                    foundedIndex = byteTypeCount - 1;
                }

                if (branchFindData.nameCount > maxNameCountData.nameCount)
                {
                    secondNameCountData = ref maxNameCountData;

                    maxNameCountData = ref branchFindData;
                    maxNameNodeFindDataIndex = foundedIndex;
                }
            }
            ref TreeNodeFindData FindData(byte targetByte, out bool isFound, out int index)
            {
                ref TreeNodeFindData nodeFindData = ref branchFindDataSpan[0];
                if (nodeFindData.keyByte == targetByte)
                {
                    isFound = true;
                    index = 0;
                    return ref nodeFindData;
                }

                for (int i = 1; i < byteTypeCount; i++)
                {
                    nodeFindData = ref branchFindDataSpan[i];

                    // 바이트가 같음
                    if (nodeFindData.keyByte == targetByte)
                    {
                        // [ 반환 ] 찾음
                        isFound = true;
                        index = i;
                        return ref nodeFindData;
                    }
                }

                // [ 반환 ] 찾지 못함
                isFound = false;
                index = -1;
                return ref nodeFindData;
            }
            
            #endregion


            #region 비공개 메서드

            ref TreeNodeFindData AddData(TreeNodeFindData branchFindData)
            {
                if (byteTypeCount >= branchFindDataSpan.Length)
                    throw new ArgumentOutOfRangeException("브랜치 데이터가 너무 많습니다.");

                ref TreeNodeFindData branchFindDataRef = ref branchFindDataSpan[byteTypeCount++];
                branchFindDataRef = branchFindData;

                return ref branchFindDataRef;
            }

            #endregion

            #endregion
        }

        public byte keyByte;
        public int nameCount;

        public TreeNodeFindData(byte keyByte, int nameCount)
        {
            this.keyByte = keyByte;
            this.nameCount = nameCount;
        }
    }
    
    class TreeNode
    {
        #region Static


        public static TreeNode? CreateTree(EventDataDescriptor[] descriptorArray)
        {
            // [ 반환 ] 비었음 -> null
            if (descriptorArray.Length == 0) {
                return null;
            }

            // [ 반환 ] 단 한 개만 존재하는 경우
            if (descriptorArray.Length == 1)
            {
                EventDataDescriptor descriptor = descriptorArray[0];
                string name = descriptor.Name;

                // 이름 길이 0
                if (name.Length == 0)
                    throw new ArgumentException("이름 길이가 0입니다.");

                return new TreeNode(descriptor);
            }

            // 설명자 인덱스 스판 생성
            ArrayIndexSpan<EventDataDescriptor> descriptorIndexSpan
                = descriptorArray.AsArrayIndexSpan(stackalloc int[descriptorArray.Length]);

            // 깊이 1 노드 탐색 세트 생성
            TreeNodeFindData.Set depth1NodeFindDataSet = GetNodeFindDataSet(
                descriptorIndexSpan, 
                0,
                stackalloc TreeNodeFindData[GetByteTypeCount(
                    descriptorIndexSpan, 
                    0, 
                    Math.Min(keyByteTypeCount, descriptorIndexSpan.Count)
                )]
            );

            // [ 반환 ] 바이트 종류가 하나 -> 그대로 다음으로 (깊이 1 노드 탐색 세트 재사용)
            if (depth1NodeFindDataSet.ByteTypeCount == 1)
            {
                TreeNodeFindData depth1FindData = depth1NodeFindDataSet[0];

                if (depth1FindData.nameCount == 1)
                {
                    // 설명자가 한 개
                    return new TreeNode(descriptorIndexSpan[0]);
                }

                return GetTreeNode(
                    descriptorIndexSpan,
                    depth1FindData.keyByte,
                    1,
                    depth1NodeFindDataSet.branchFindDataSpan
                );
            }
            else
            {
                // 깊이 2 노드 탐색용 버퍼
                Span<TreeNodeFindData> depth2NodeFindDataBuffer 
                    = stackalloc TreeNodeFindData[depth1NodeFindDataSet.SecondNameCountData.nameCount];

                // 다음 설명자 ArrayIndexSpan 생성용 버퍼
                Span<int> indexBuffer 
                    = stackalloc int[depth1NodeFindDataSet.MaxNameCountData.nameCount];

                TreeNode[] branchArray = new TreeNode[depth1NodeFindDataSet.ByteTypeCount];
                for (int branchIndex = 0; branchIndex < branchArray.Length; branchIndex++)
                {
                    if (branchIndex == depth1NodeFindDataSet.MaxNameCountDataIndex)
                    {
                        // 최대 이름 개수 노드 탐색 세트 재사용 예정
                        continue;
                    }

                    ref TreeNodeFindData depth1FindData = ref depth1NodeFindDataSet[branchIndex];

                    ArrayIndexSpan<EventDataDescriptor> nextNodeFindData = new (
                        descriptorIndexSpan.originalValueSpan, 
                        indexBuffer
                    );

                    ArrayIndexSpan<EventDataDescriptor> extractedOriginalIndexSpan = new (
                        descriptorIndexSpan.originalValueSpan, 
                        descriptorIndexSpan.indexSpan
                    );

                    branchArray[branchIndex] = ExtractDescriptorIndexSpanAndGetNode(
                        ref descriptorIndexSpan, 
                        nextNodeFindData, 
                        extractedOriginalIndexSpan,
                        depth1FindData, 
                        depth2NodeFindDataBuffer
                    );
                }

                // 가장 이름 인덱스 버퍼 크기를 많이 소모하는 노드는 깊이 1 탐색 세트 재사용
                branchArray[depth1NodeFindDataSet.MaxNameCountDataIndex] = GetTreeNode(
                    descriptorIndexSpan,
                    depth1NodeFindDataSet.MaxNameCountData.keyByte,
                    1,
                    depth1NodeFindDataSet.branchFindDataSpan
                );

                return new TreeNode(0, 0, branchArray, null);
            }
        }

        static TreeNode GetTreeNode(ArrayIndexSpan<EventDataDescriptor> descriptorIndexSpan, byte keyByte, int findNameIndex,
            Span<TreeNodeFindData> nodeFindDataBuffer)
        {
            TreeNodeFindData.Set nodeFindDataSet = GetNodeFindDataSet(
                descriptorIndexSpan, 
                findNameIndex,
                nodeFindDataBuffer
            );

            // 종류 한 개
            if (nodeFindDataSet.ByteTypeCount == 1)
            {
                TreeNodeFindData nodeFindData = nodeFindDataSet[0];

                // 설명자가 한 개
                if (nodeFindData.nameCount == 1)
                {
                    return new TreeNode(
                        descriptorIndexSpan[0]
                    );
                }
                else
                {
                    // 그대로 다음 인덱스 탐색 (노드 탐색 세트 재사용)
                    return GetTreeNode(
                        descriptorIndexSpan, 
                        keyByte,
                        findNameIndex + 1, 
                        nodeFindDataSet.branchFindDataSpan
                    );
                }
            }
            // 종류 여러 개
            else
            {
                // 다음 깊이 노드 탐색용 버퍼
                Span<TreeNodeFindData> nextNodeFindDataBuffer 
                    = stackalloc TreeNodeFindData[nodeFindDataSet.SecondNameCountData.nameCount];

                // 다음 설명자 ArrayIndexSpan 생성용 버퍼
                Span<int> indexBuffer 
                    = stackalloc int[nodeFindDataSet.MaxNameCountData.nameCount];

                // 하위 노드 배열 생성
                TreeNode[] branchArray = new TreeNode[nodeFindDataSet.ByteTypeCount];
                for (int branchIndex = 0; branchIndex < branchArray.Length; branchIndex++)
                {
                    if (branchIndex == nodeFindDataSet.MaxNameCountDataIndex)
                    {
                        // 최대 이름 개수 노드 탐색 세트 재사용 예정
                        continue;
                    }
                    ref TreeNodeFindData nodeFindData = ref nodeFindDataSet[branchIndex];

                    // 설명자가 1개
                    if (nodeFindData.nameCount == 1)
                    {
                        // 대상 설명자 찾기
                        EventDataDescriptor descriptor = FindFirstKeyByteSameDescriptor(
                            descriptorIndexSpan, 
                            findNameIndex, 
                            nodeFindData.keyByte
                        );

                        // 하위 값 노드 생성
                        branchArray[branchIndex] = new TreeNode(
                            descriptor
                        );
                    }
                    // 설명자가 여러 개
                    else
                    {
                        // 설명자 추출
                        ArrayIndexSpan<EventDataDescriptor> nextNodeFindData = new (
                            descriptorIndexSpan.originalValueSpan, 
                            indexBuffer
                        );

                        ArrayIndexSpan<EventDataDescriptor> extractedOriginalIndexSpan = new (
                            descriptorIndexSpan.originalValueSpan, 
                            descriptorIndexSpan.indexSpan
                        );

                        for (int i = 0; i < descriptorIndexSpan.Count; i++)
                        {
                            int descriptorIndex = descriptorIndexSpan.GetIndex(i);
                            
                            EventDataDescriptor descriptor = descriptorIndexSpan.GetValue(descriptorIndex);
                            string descriptorName = descriptor.Name;

                            // 이름 길이 초과
                            if (findNameIndex + 1 >= descriptorName.Length) continue;

                            // 바이트가 같음 -> 결과에 추가
                            if (descriptorName[findNameIndex + 1] == nodeFindData.keyByte)
                            {
                                nextNodeFindData.Add(descriptorIndex);
                            }
                            // 바이트가 다름 -> 남겨둠
                            else
                            {
                                extractedOriginalIndexSpan.Add(descriptorIndex);
                            }
                        }


                        // 원본에 적용
                        descriptorIndexSpan = extractedOriginalIndexSpan;

                        // 하위 노드 생성 : 다음 인덱스 탐색
                        branchArray[branchIndex] = GetTreeNode(
                            nextNodeFindData,
                            nodeFindData.keyByte,
                            findNameIndex + 1, 
                            nextNodeFindDataBuffer
                        );
                    }
                }
                
                // 가장 이름 인덱스 버퍼 크기를 많이 소모하는 노드는 원본 노드 탐색 세트 재사용
                branchArray[nodeFindDataSet.MaxNameCountDataIndex] = GetTreeNode(
                    descriptorIndexSpan, 
                    nodeFindDataSet.MaxNameCountData.keyByte,
                    findNameIndex + 1, 
                    nodeFindDataSet.branchFindDataSpan
                );

                return new TreeNode(
                    keyByte, 
                    findNameIndex, 
                    branchArray, 
                    null
                );
            }
        }

        static TreeNode ExtractDescriptorIndexSpanAndGetNode(ref ArrayIndexSpan<EventDataDescriptor> descriptorIndexSpan,
            ArrayIndexSpan<EventDataDescriptor> nextDescriptorIndexSpan, 
            ArrayIndexSpan<EventDataDescriptor> extractedOriginalIndexSpan, 
            TreeNodeFindData depth1FindData, 
            Span<TreeNodeFindData> nextNodeFindDataBuffer)
        {
            for (int descriptorIndexIndex = 0; descriptorIndexIndex < descriptorIndexSpan.Count; descriptorIndexIndex++)
            {
                int descriptorIndex = descriptorIndexSpan.GetIndex(descriptorIndexIndex);
                
                EventDataDescriptor descriptor = descriptorIndexSpan.GetValue(descriptorIndex);
                string descriptorName = descriptor.Name;

                // 이름 길이 초과
                if (1 >= descriptorName.Length) continue;

                // 바이트가 같음 -> 결과에 추가
                if (descriptorName[1] == depth1FindData.keyByte)
                {
                    nextDescriptorIndexSpan.Add(descriptorIndex);
                }
                // 바이트가 다름 -> 남겨둠
                else
                {
                    extractedOriginalIndexSpan.Add(descriptorIndex);
                }
            }

            // 원본에 적용
            descriptorIndexSpan = extractedOriginalIndexSpan;

            // 하위 노드 생성 : 다음 인덱스 탐색
            return GetTreeNode(
                nextDescriptorIndexSpan, 
                depth1FindData.keyByte, 
                1, 
                nextNodeFindDataBuffer
            );
        }
        
        static EventDataDescriptor FindFirstKeyByteSameDescriptor(ArrayIndexSpan<EventDataDescriptor> descriptorIndexSpan, int nameIndex, byte keyByte)
        {
            for (int descriptorIndex = 0; descriptorIndex < descriptorIndexSpan.Count; descriptorIndex++)
            {
                EventDataDescriptor descriptor = descriptorIndexSpan[descriptorIndex];

                if (descriptor.Name[nameIndex] == keyByte)
                {
                    return descriptor;
                }
            }
            throw new Exception("설명자를 찾을 수 없습니다.");
        }

        static int GetByteTypeCount(ArrayIndexSpan<EventDataDescriptor> descriptorIndexSpan, int findNameIndex, int maxCount)
        {
            TreeNodeFindData.Set nodeFindDataSet = new (
                stackalloc TreeNodeFindData[maxCount], 
                findNameIndex
            );

            for (int i = 0; i < descriptorIndexSpan.Count; i++)
            {
                EventDataDescriptor descriptor = descriptorIndexSpan[i];
                nodeFindDataSet.AddValue(descriptor);
            }
            return nodeFindDataSet.ByteTypeCount;
        }

        static TreeNodeFindData.Set GetNodeFindDataSet(ArrayIndexSpan<EventDataDescriptor> descriptorIndexSpan, 
            int findNameIndex,
            Span<TreeNodeFindData> nodeFindDataBuffer)
        {
            TreeNodeFindData.Set result = new (nodeFindDataBuffer, findNameIndex);
            for (int i = 0; i < descriptorIndexSpan.Count; i++)
            {
                EventDataDescriptor descriptor = descriptorIndexSpan[i];
                result.AddValue(descriptor);
            }
            return result;
        }

        #endregion


        #region Instance
        
        public byte keyStartByte;
        public int branchKeyStartIndex;
        public TreeNode[]? branchArray;
        public EventDataDescriptor? value;

        public TreeNode(byte startByte, int branchKeyStartIndex, TreeNode[]? branchArray, EventDataDescriptor? descriptor)
        {
            this.keyStartByte = startByte;
            this.branchKeyStartIndex = branchKeyStartIndex;
            this.branchArray = branchArray;
            this.value = descriptor;
        }
        /// <summary>
        /// 값만 존재하는 노드를 생성합니다.
        /// </summary>
        /// <param name="descriptor"></param>
        public TreeNode(EventDataDescriptor descriptor)
        {
            keyStartByte = 0;
            branchKeyStartIndex = -1;
            branchArray = null;
            value = descriptor;
        }

        public EventDataDescriptor? GetDescriptor(string name)
        {
            // [ 반환 ] 값이 존재하는 노드
            if (value != null)
            {
                if (value.Name.Length == name.Length)
                {
                    return value;
                }
            }

            if (branchArray == null) {
                JohwaLogger.Log("브랜치가 없습니다.");
                return null;
            }
            for (int i = 0; i < branchArray.Length; i++)
            {
                TreeNode branchNode = branchArray[i];
                if (branchNode.keyStartByte == name[branchKeyStartIndex])
                {
                    return branchNode.GetDescriptor(name);
                }
            }

            throw new Exception("설명자를 찾을 수 없습니다.");
        }

        #endregion
    }

    #endregion


    #region Static

    public const int keyByteTypeCount = ('Z' - 'A' + 1) + 1;

    #endregion


    #region Instance

    // 필드

    EventDataDescriptor[] descriptorArray;
    TreeNode? treeRoot;
    

    // 생성자
    public EventDataDescriptorTree(EventDataDescriptor[] descriptorArray)
    {
        this.descriptorArray = descriptorArray;
        this.treeRoot = TreeNode.CreateTree(descriptorArray);
    }

    public EventDataDescriptor? GetDescriptor(string name)
    {
        if (treeRoot == null) {
            JohwaLogger.Log("트리가 비었습니다.");
            return null;
        }

        return treeRoot.GetDescriptor(name);
    }
    
    #endregion
}