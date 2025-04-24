using Johwa.Common.Extension.Johwa.Common.Collection;

namespace Johwa.Common.Collection;

public class ByteSpanTree<TValue>
{
    #region Object

    struct TreeNodeFindData
    {
        public ref struct Set
        {
            // 필드 & 프로퍼티
            public Span<TreeNodeFindData> nodeFindDataSpan;
            public readonly int findByteIndex;

            public int ByteTypeCount => byteTypeCount;
            public ref TreeNodeFindData MaxByteSpanLengthData => ref maxByteSpanLengthData;
            public int MaxByteSpanLengthDataIndex => maxByteSpanLengthDataIndex;
            public ref TreeNodeFindData SecondByteSpanLengthData => ref secondByteSpanLengthData;

            int byteTypeCount;
            ref TreeNodeFindData maxByteSpanLengthData;
            int maxByteSpanLengthDataIndex;
            ref TreeNodeFindData secondByteSpanLengthData;


            // 생성자
            public Set(Span<TreeNodeFindData> childFindDataBuffer, int findByteIndex)
            {
                this.nodeFindDataSpan = childFindDataBuffer;
                this.byteTypeCount = 0;
                this.findByteIndex = findByteIndex;
            }
            

            #region 메서드

            #region 공개 메서드
            
            public ref TreeNodeFindData this[int index] => ref Get(index);
            public ref TreeNodeFindData Get(int index)
            {
                if (index < 0 || index >= byteTypeCount)
                    throw new ArgumentOutOfRangeException("인덱스가 범위를 벗어났습니다.");

                return ref nodeFindDataSpan[index];
            }
            
            public void AddByte(byte keyByte)
            {
                if (byteTypeCount >= nodeFindDataSpan.Length)
                    throw new ArgumentOutOfRangeException("브랜치 데이터가 너무 많습니다.");

                if (byteTypeCount == 0)
                {
                    AddData(new TreeNodeFindData(
                        keyByte, 
                        1
                    ));
                    return;
                }

                bool isFound;
                int foundedIndex;

                ref TreeNodeFindData branchFindData = ref FindData(
                    keyByte, 
                    out isFound,
                    out foundedIndex
                );

                if (isFound)
                {
                    branchFindData.valueCount++;
                }
                else
                {
                    branchFindData = ref AddData(new TreeNodeFindData(
                        keyByte, 
                        1
                    ));
                    foundedIndex = byteTypeCount - 1;
                }

                if (branchFindData.valueCount > maxByteSpanLengthData.valueCount)
                {
                    secondByteSpanLengthData = ref maxByteSpanLengthData;

                    maxByteSpanLengthData = ref branchFindData;
                    maxByteSpanLengthDataIndex = foundedIndex;
                }
            }
            ref TreeNodeFindData FindData(byte targetByte, out bool isFound, out int index)
            {
                ref TreeNodeFindData nodeFindData = ref nodeFindDataSpan[0];
                if (nodeFindData.keyByte == targetByte)
                {
                    isFound = true;
                    index = 0;
                    return ref nodeFindData;
                }

                for (int i = 1; i < byteTypeCount; i++)
                {
                    nodeFindData = ref nodeFindDataSpan[i];

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
                if (byteTypeCount >= nodeFindDataSpan.Length)
                    throw new ArgumentOutOfRangeException("브랜치 데이터가 너무 많습니다.");

                ref TreeNodeFindData branchFindDataRef = ref nodeFindDataSpan[byteTypeCount++];
                branchFindDataRef = branchFindData;

                return ref branchFindDataRef;
            }

            #endregion

            #endregion
        }

        public byte keyByte;
        public int valueCount;

        public TreeNodeFindData(byte keyByte, int valueCount)
        {
            this.keyByte = keyByte;
            this.valueCount = valueCount;
        }
    }
    
    class TreeNode
    {
        #region Instance
        
        public byte keyStartByte;
        public int childNodeKeyStartIndex;

        public TValue? value;
        public bool hasValue;
        public TreeNode[]? childNodeArray;

        public TreeNode(byte startByte, int childNodeKeyStartIndex, TValue? value, bool hasValue, TreeNode[]? childNodeArray)
        {
            this.keyStartByte = startByte;
            this.childNodeKeyStartIndex = childNodeKeyStartIndex;
            this.value = value;
            this.hasValue = hasValue;
            this.childNodeArray = childNodeArray;
        }

        /// <summary>
        /// 값만 존재하는 노드를 생성합니다.
        /// </summary>
        /// <param name="descriptor"></param>
        public TreeNode(TValue descriptor)
        {
            keyStartByte = 0;
            childNodeKeyStartIndex = -1;
            childNodeArray = null;
            value = descriptor;
            hasValue = true;
        }

        public TValue? GetValue(ReadOnlySpan<byte> byteSpan, ByteSpanTree<TValue> tree)
        {
            // 값이 존재하는 노드이고 길이가 동일하면 반환
            if (value != null)
            {
                int byteSpanLength = tree.getByteSpanLengthFunc.Invoke(value);

                if (byteSpanLength == byteSpan.Length)
                {
                    return value;
                }
            }

            // 하위 노드가 없음 -> null 반환
            if (childNodeArray == null) {
                return default;
            }

            for (int i = 0; i < childNodeArray.Length; i++)
            {
                TreeNode childNode = childNodeArray[i];
                if (childNode.keyStartByte == byteSpan[childNodeKeyStartIndex])
                {
                    return childNode.GetValue(byteSpan, tree);
                }
            }

            throw new Exception("설명자를 찾을 수 없습니다.");
        }

        #endregion
    }

    #endregion


    #region Static

    static TreeNode? CreateTree(TValue[] valueArray, ByteSpanTree<TValue> tree)
    {
        // 비었음 -> null 반환
        if (valueArray.Length == 0) 
        {
            return null;
        }

        // 단 한 개만 존재하는 경우 -> 값 노드로 반환
        if (valueArray.Length == 1)
        {
            return new TreeNode(valueArray[0]);
        }

        // 설명자 인덱스 스판 생성
        ArrayIndexSpan<TValue> valueIndexSpan
            = valueArray.AsArrayIndexSpan(stackalloc int[valueArray.Length]);

        int valueIndexSpanByteTypeCount 
            = GetByteTypeCount(
                valueIndexSpan, 
                0, 
                tree
            );

        // 깊이 1 노드 탐색 세트 생성
        TreeNodeFindData.Set depth1NodeFindDataSet = GetNodeFindDataSet(
            valueIndexSpan, 
            0,
            stackalloc TreeNodeFindData[valueIndexSpanByteTypeCount]
        );

        // 바이트 종류가 하나 -> 그대로 다음으로 (깊이 1 노드 탐색 세트 재사용)
        if (depth1NodeFindDataSet.ByteTypeCount == 1)
        {
            TreeNodeFindData depth1FindData = depth1NodeFindDataSet[0];

            // 설명자가 1개 -> 값 노드 반환
            if (depth1FindData.valueCount == 1)
            {
                return new TreeNode(valueIndexSpan[0]);
            }
            // 설명자가 여러 개
            else
            {
                return GetTreeNode(
                    valueIndexSpan,
                    depth1FindData.keyByte,
                    1,
                    depth1NodeFindDataSet.nodeFindDataSpan,
                    tree
                );
            }
        }
        else
        {
            int depth2NodeFindDataMaxByteTypeCount 
                = GetMaxByteTypeCount(
                    valueIndexSpan, 
                    1, 
                    tree
                );

            // 깊이 2 노드 탐색용 버퍼
            Span<TreeNodeFindData> depth2NodeFindDataBuffer 
                = stackalloc TreeNodeFindData[depth2NodeFindDataMaxByteTypeCount];

            // 다음 설명자 ArrayIndexSpan 생성용 버퍼
            Span<int> indexBuffer 
                = stackalloc int[depth1NodeFindDataSet.SecondByteSpanLengthData.valueCount];

            TreeNode[] depth1NodeArray = new TreeNode[depth1NodeFindDataSet.ByteTypeCount];
            for (int depth1NodeIndex = 0; depth1NodeIndex < depth1NodeArray.Length; depth1NodeIndex++)
            {
                // 최대 이름 개수 노드 -> 기존 인덱스 버퍼 재사용 예정
                if (depth1NodeIndex == depth1NodeFindDataSet.MaxByteSpanLengthDataIndex)
                {
                    continue;
                }

                ref TreeNodeFindData depth1FindData = ref depth1NodeFindDataSet[depth1NodeIndex];

                ArrayIndexSpan<TValue> nextNodeFindData = new (
                    valueIndexSpan.originalValueSpan, 
                    indexBuffer
                );

                ArrayIndexSpan<TValue> extractedOriginalIndexSpan = new (
                    valueIndexSpan.originalValueSpan, 
                    valueIndexSpan.indexSpan
                );

                depth1NodeArray[depth1NodeIndex] = ExtractDescriptorIndexSpanAndGetNode(
                    ref valueIndexSpan, 
                    nextNodeFindData, 
                    extractedOriginalIndexSpan,
                    depth1FindData, 
                    depth2NodeFindDataBuffer,
                    tree
                );
            }

            // 가장 이름 인덱스 버퍼 크기를 많이 소모하는 노드는 기존 인덱스 버퍼 재사용
            depth1NodeArray[depth1NodeFindDataSet.MaxByteSpanLengthDataIndex] = GetTreeNode(
                valueIndexSpan,
                depth1NodeFindDataSet.MaxByteSpanLengthData.keyByte,
                1,
                depth2NodeFindDataBuffer,
                tree
            );

            return new TreeNode(
                0, 
                0, 
                default, 
                false, 
                depth1NodeArray
            );
        }
    }

    static TreeNode GetTreeNode(ArrayIndexSpan<TValue> valueIndexSpan, byte keyByte, int findByteIndex,
        Span<TreeNodeFindData> nodeFindDataBuffer,
        Span<int> valueIndexBuffer,
        ByteSpanTree<TValue> tree)
    {
        TreeNodeFindData.Set nodeFindDataSet = GetNodeFindDataSet(
            valueIndexSpan, 
            findByteIndex,
            nodeFindDataBuffer
        );

        // 종류 한 개
        if (nodeFindDataSet.ByteTypeCount == 1)
        {
            TreeNodeFindData nodeFindData = nodeFindDataSet[0];

            // 설명자가 한 개
            if (nodeFindData.valueCount == 1)
            {
                return new TreeNode(
                    valueIndexSpan[0]
                );
            }
            else
            {
                // 그대로 다음 인덱스 탐색 (노드 탐색 세트 재사용)
                return GetTreeNode(
                    valueIndexSpan, 
                    keyByte,
                    findByteIndex + 1, 
                    nodeFindDataSet.nodeFindDataSpan,
                    tree
                );
            }
        }
        // 종류 여러 개
        else
        {
            int nextNodeFindDataMaxByteTypeCount 
                = GetMaxByteTypeCount(
                    valueIndexSpan, 
                    findByteIndex + 1, 
                    tree
                );
            // 다음 깊이 노드 탐색용 버퍼
            Span<TreeNodeFindData> nextNodeFindDataBuffer 
                = stackalloc TreeNodeFindData[nextNodeFindDataMaxByteTypeCount];

            // 다음 설명자 ArrayIndexSpan 생성용 버퍼
            Span<int> nextNodeFindDataIndexBuffer 
                = stackalloc int[nodeFindDataSet.SecondByteSpanLengthData.valueCount];

            // 하위 노드 배열 생성
            TreeNode[] childNodeArray = new TreeNode[nodeFindDataSet.ByteTypeCount];
            for (int childNodeIndex = 0; childNodeIndex < childNodeArray.Length; childNodeIndex++)
            {
                if (childNodeIndex == nodeFindDataSet.MaxByteSpanLengthDataIndex)
                {
                    // 최대 이름 개수 노드 탐색 세트 재사용 예정
                    continue;
                }
                ref TreeNodeFindData nodeFindData = ref nodeFindDataSet[childNodeIndex];

                // 설명자가 1개
                if (nodeFindData.valueCount == 1)
                {
                    // 대상 설명자 찾기
                    TValue descriptor = FindFirstKeyByteSameValue(
                        valueIndexSpan, 
                        findByteIndex, 
                        nodeFindData.keyByte,
                        tree
                    );

                    // 하위 노드 생성 (값만 존재)
                    childNodeArray[childNodeIndex] = new TreeNode(descriptor);
                }
                // 설명자가 여러 개
                else
                {
                    // 설명자 추출
                    ArrayIndexSpan<TValue> nextNodeFindData = new (
                        valueIndexSpan.originalValueSpan, 
                        nextNodeFindDataIndexBuffer
                    );

                    ArrayIndexSpan<TValue> extractedOriginalIndexSpan = new (
                        valueIndexSpan.originalValueSpan, 
                        valueIndexSpan.indexSpan
                    );

                    for (int i = 0; i < valueIndexSpan.Count; i++)
                    {
                        int valueIndex = valueIndexSpan.GetIndex(i);
                        
                        TValue value = valueIndexSpan.GetValue(valueIndex);
                        int valueByteSpanLength = tree.getByteSpanLengthFunc(value);
                        int nextValueByteIndex = findByteIndex + 1;

                        // 이름 길이 초과
                        if (nextValueByteIndex >= valueByteSpanLength) continue;

                        byte nextValueKeyByte = tree.getByteFunc.Invoke(value, nextValueByteIndex);

                        // 바이트가 같음 -> 결과에 추가
                        if (nextValueKeyByte == nodeFindData.keyByte)
                        {
                            nextNodeFindData.Add(valueIndex);
                        }
                        // 바이트가 다름 -> 남겨둠
                        else
                        {
                            extractedOriginalIndexSpan.Add(valueIndex);
                        }
                    }

                    // 원본에 적용
                    valueIndexSpan = extractedOriginalIndexSpan;

                    // 하위 노드 생성 : 다음 인덱스 탐색
                    childNodeArray[childNodeIndex] = GetTreeNode(
                        nextNodeFindData,
                        nodeFindData.keyByte,
                        findByteIndex + 1, 
                        nextNodeFindDataBuffer,
                        tree
                    );
                }
            }
            
            // 가장 이름 인덱스 버퍼 크기를 많이 소모하는 노드는 원본 인덱스 버퍼 재사용
            childNodeArray[nodeFindDataSet.MaxByteSpanLengthDataIndex] = GetTreeNode(
                valueIndexSpan, 
                nodeFindDataSet.MaxByteSpanLengthData.keyByte,
                findByteIndex + 1, 
                nodeFindDataSet.nodeFindDataSpan,
                valueIndexBuffer,
                tree
            );

            return new TreeNode(
                keyByte, 
                findByteIndex, 
                default,
                false,
                childNodeArray
            );
        }
    }

    static TreeNode ExtractDescriptorIndexSpanAndGetNode(ref ArrayIndexSpan<TValue> valueIndexSpan,
        ArrayIndexSpan<TValue> nextValueIndexSpan, 
        ArrayIndexSpan<TValue> extractedOriginalIndexSpan, 
        TreeNodeFindData depth1FindData, 
        Span<TreeNodeFindData> nextNodeFindDataBuffer, 
        ByteSpanTree<TValue> tree)
    {
        for (int valueIndexIndex = 0; valueIndexIndex < valueIndexSpan.Count; valueIndexIndex++)
        {
            int valueIndex = valueIndexSpan.GetIndex(valueIndexIndex);
            
            TValue descriptor = valueIndexSpan.GetValue(valueIndex);

            byte keyByte = tree.getByteFunc.Invoke(descriptor, depth1FindData.);

            // 바이트가 같음 -> 결과에 추가
            if (keyByte == depth1FindData.keyByte)
            {
                nextValueIndexSpan.Add(valueIndex);
            }
            // 바이트가 다름 -> 남겨둠
            else
            {
                extractedOriginalIndexSpan.Add(valueIndex);
            }
        }

        // 원본에 적용
        valueIndexSpan = extractedOriginalIndexSpan;

        // 하위 노드 생성 : 다음 인덱스 탐색
        return GetTreeNode(
            nextValueIndexSpan, 
            depth1FindData.keyByte, 
            1, 
            nextNodeFindDataBuffer,
            tree
        );
    }
    
    static TValue FindFirstKeyByteSameValue(ArrayIndexSpan<TValue> descriptorIndexSpan, int nameIndex, byte keyByte, 
        ByteSpanTree<TValue> tree)
    {
        for (int descriptorIndex = 0; descriptorIndex < descriptorIndexSpan.Count; descriptorIndex++)
        {
            TValue value = descriptorIndexSpan[descriptorIndex];

            if (tree.getByteFunc(value, nameIndex) == keyByte)
            {
                return value;
            }
        }
        throw new Exception("설명자를 찾을 수 없습니다.");
    }

    static int GetByteTypeCount(ArrayIndexSpan<TValue> valueIndexSpan, int findByteIndex, ByteSpanTree<TValue> tree)
    {
        int maxCount = Math.Min(tree.maxByteTypeCount, valueIndexSpan.Count);

        TreeNodeFindData.Set nodeFindDataSet = new (
            stackalloc TreeNodeFindData[maxCount], 
            findByteIndex
        );

        for (int i = 0; i < valueIndexSpan.Count; i++)
        {
            TValue value = valueIndexSpan[i];
            byte keyByte = tree.getByteFunc(value, findByteIndex);

            nodeFindDataSet.AddByte(keyByte);
        }
        return nodeFindDataSet.ByteTypeCount;
    }

    static TreeNodeFindData.Set GetNodeFindDataSet(ArrayIndexSpan<TValue> descriptorIndexSpan, 
        int findByteIndex,
        Span<TreeNodeFindData> nodeFindDataBuffer)
    {
        TreeNodeFindData.Set result = new (nodeFindDataBuffer, findByteIndex);
        for (int i = 0; i < descriptorIndexSpan.Count; i++)
        {
            TValue descriptor = descriptorIndexSpan[i];
            result.AddValue(descriptor);
        }
        return result;
    }

    #endregion


    #region Instance

    // 필드

    
    TValue[] descriptorArray;
    TreeNode? treeRoot;
    /// <summary>
    /// (바이트를 추출할 값, 추출할 바이트 인덱스) -> 반환할 바이트
    /// </summary>
    Func<TValue, int, byte> getByteFunc;

    /// <summary>
    /// (바이트를 추출할 값) -> 반환할 바이트 길이
    /// </summary>
    Func<TValue, int> getByteSpanLengthFunc;
    int maxByteTypeCount = 256;

    // 생성자
    public ByteSpanTree(TValue[] descriptorArray, 
        Func<TValue, int, byte> getByteFunc, Func<TValue, int> getByteSpanLengthFunc, int maxByteTypeCount = 256)
    {
        this.descriptorArray = descriptorArray;
        this.getByteFunc = getByteFunc;
        this.getByteSpanLengthFunc = getByteSpanLengthFunc;
        this.maxByteTypeCount = maxByteTypeCount;
        this.treeRoot = CreateTree(descriptorArray, this);
    }

    public TValue? GetValue(ReadOnlySpan<byte> byteSpan)
    {
        if (treeRoot == null) {
            return default;
        }

        return treeRoot.GetValue(byteSpan, this);
    }
    
    #endregion
}