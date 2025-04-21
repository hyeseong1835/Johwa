using System.Buffers;
using Johwa.Common.Collection;

namespace Johwa.Event.Data;

public class EventDataDescriptorDictionary
{
    #region Object

    public ref struct TreeNodeDataSet
    {
        Span<TreeNodeData> nodeDataSpan;
        public int Count { get; private set; }

        public TreeNodeDataSet(Span<TreeNodeData> nodeDataSpan)
        {
            this.nodeDataSpan = nodeDataSpan;
            this.Count = 0;
        }
        public ref TreeNodeData this[int index]
           => ref Get(index);

        public ref TreeNodeData Get(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("인덱스가 범위를 벗어났습니다.");

            return ref nodeDataSpan[index];
        }
        public void Add(TreeNodeData nodeData)
        {
            if (Count >= nodeDataSpan.Length)
                throw new ArgumentOutOfRangeException("노드 데이터가 너무 많습니다.");

            nodeDataSpan[Count++] = nodeData;
        }
    }
    unsafe public struct TreeNodeData
    {
        public byte keyStartByte;
        public byte curKeyByte;
        public int keyStartIndex;
        public fixed int nameIndexArray[256];
        public int nameIndexCount;

        public TreeNodeData(byte keyByte, int keyStartIndex)
        {
            this.keyStartByte = keyByte;
            this.curKeyByte = keyByte;
            this.keyStartIndex = keyStartIndex;
            this.nameIndexCount = 0;
        }

        public void AddName(int nameIndex)
        {
            if (nameIndexCount >= 256)
                throw new ArgumentOutOfRangeException("이름이 너무 많습니다.");

            nameIndexArray[nameIndexCount++] = nameIndex;
        }
    }
    public struct KeyByteData
    {
        public byte keyByte;
        public ArrayIndexSet<string> nameIndexSet;

        public KeyByteData(byte keyByte, ArrayIndexSet<string> nameIndexSet)
        {
            this.keyByte = keyByte;
            this.nameIndexSet = nameIndexSet;
        }

        public void Reset(string[] originalNameArray, int maxNameCount)
        {
            if (keyByte == 0)
            {
                nameIndexSet = new ArrayIndexSet<string>(originalNameArray, maxNameCount);
            }
            else 
            {
                keyByte = 0;
                nameIndexSet.Reset(maxNameCount);
            }
        }
        public void AddNameIndex(int nameIndex)
        {
            nameIndexSet.Add(nameIndex);
        }
    }
    public class KeyByteDataSet
    {
        public string[] originalNameArray;
        public KeyByteData[] setArray;
        public int keyByteSetCount;

        public KeyByteDataSet(string[] originalNameArray, KeyByteData[] setArray)
        {
            this.originalNameArray = originalNameArray;
            this.setArray = setArray;
            this.keyByteSetCount = 0;
        }
        public void Reset()
        {
            keyByteSetCount = 0;
        }

        public ref KeyByteData this[int index]
            => ref Get(index);
        public ref KeyByteData Get(int index)
        {
            if (index < 0 || index >= keyByteSetCount)
                throw new ArgumentOutOfRangeException("인덱스가 범위를 벗어났습니다.");

            return ref setArray[index];
        }
        public void Add(byte keyByte, int nameIndex, int maxNameCount)
        {
            for (int i = 0; i < keyByteSetCount; i++)
            {
                ref KeyByteData set = ref setArray[i];

                if (set.keyByte == keyByte)
                {
                    set.AddNameIndex(nameIndex);
                    return;
                }
            }
            ref KeyByteData newSet = ref AddData(keyByte, maxNameCount);
            newSet.AddNameIndex(nameIndex);
        }
        ref KeyByteData AddData(byte keyByte, int maxNameCount)
        {
            if (keyByteSetCount >= setArray.Length)
                throw new ArgumentOutOfRangeException("키 바이트가 너무 많습니다.");

            ref KeyByteData newSetRef = ref setArray[keyByteSetCount];
            newSetRef.Reset(originalNameArray, maxNameCount);
            newSetRef.keyByte = keyByte;

            return ref newSetRef;
        }
    }
    public struct TreeNodeFindData
    {
        public ref struct Set
        {
            public Span<TreeNodeFindData> branchFindDataSpan;
            public int count;
            public int Count => count;
            public int nameKeyFindIndex;

            public Set(Span<TreeNodeFindData> branchFindDataSpan, int nameKeyFindIndex)
            {
                this.branchFindDataSpan = branchFindDataSpan;
                this.count = 0;
                this.nameKeyFindIndex = nameKeyFindIndex;
            }
            public Set ExtractSet(Span<TreeNodeFindData> newFindDataSpan, int nameKeyFindIndex)
            {
                if (newFindDataSpan.Length < count)
                    throw new ArgumentOutOfRangeException("브랜치 데이터가 너무 많습니다.");

                Set set = new Set(newFindDataSpan, nameKeyFindIndex);
                for (int i = 0; i < count; i++)
                {
                    TreeNodeFindData branchFindData = branchFindDataSpan[i];
                    set.branchFindDataSpan[i] = branchFindData;
                }

                return set;
            }
            public void Reset(int nameKeyFindIndex)
            {
                this.nameKeyFindIndex = nameKeyFindIndex;
                this.count = 0;
            }
            public ref TreeNodeFindData this[int index]
                => ref Get(index);
            public ref TreeNodeFindData Get(int index)
            {
                if (index < 0 || index >= count)
                    throw new ArgumentOutOfRangeException("인덱스가 범위를 벗어났습니다.");

                return ref branchFindDataSpan[index];
            }
            public void Add(TreeNodeFindData branchFindData)
            {
                if (count >= branchFindDataSpan.Length)
                    throw new ArgumentOutOfRangeException("브랜치 데이터가 너무 많습니다.");

                branchFindDataSpan[count++] = branchFindData;
            }
            public void AddValue(EventDataDescriptor descriptor)
            {
                if (count >= branchFindDataSpan.Length)
                    throw new ArgumentOutOfRangeException("브랜치 데이터가 너무 많습니다.");

                for (int i = 0; i < count; i++)
                {
                    ref TreeNodeFindData branchFindData = ref branchFindDataSpan[i];
                    if (branchFindData.keyByte == (byte)descriptor.Name[0])
                    {
                        branchFindData.nameCount++;
                        return;
                    }
                }
                TreeNodeFindData branchFindData = new TreeNodeFindData((byte)descriptor.Name[0]);
                branchFindData.nameCount = 1;
                branchFindDataSpan[count++] = branchFindData;
            }
        }

        public byte keyByte;
        public int nameCount;

        public TreeNodeFindData(byte keyByte)
        {
            this.keyByte = keyByte;
            this.nameCount = 0;
        }
    }
    public class TreeNodeRoot
    {
        public TreeNode[]? branchArray;
        public EventDataDescriptor? value;

        public TreeNodeRoot(EventDataDescriptor[] descriptorArray, int keyByteTypeCount)
        {
            if (descriptorArray.Length == 0)
            {
                branchArray = null;
                value = null;
                return;
            }

            if (descriptorArray.Length == 1)
            {
                EventDataDescriptor descriptor = descriptorArray[0];
                string name = descriptor.Name;
                if (name.Length == 0)
                {
                    branchArray = null;
                    value = null;
                    return;
                }

                branchArray = null;
                value = descriptor;
                return;
            }


            // 공용 노드 탐색 세트
            int maxBranchCount = Math.Min(keyByteTypeCount, descriptorArray.Length);
            TreeNodeFindData.Set nodeFindDataSet = new (stackalloc TreeNodeFindData[maxBranchCount], 0);


            // 설명자 탐색
            for (int i = 0; i < descriptorArray.Length; i++)
            {
                EventDataDescriptor descriptor = descriptorArray[i];
                if (descriptor.Name.Length == 0) continue;

                // 브랜치 탐색 데이터 생성
                TreeNodeFindData branchFindData = new TreeNodeFindData((byte)descriptor.Name[0]);

                nodeFindDataSet.Add(branchFindData);
            }

            // 탐색 결과 추출
            TreeNodeFindData.Set branchFindDataSet = nodeFindDataSet.ExtractSet(stackalloc TreeNodeFindData[nodeFindDataSet.count], 0);

            // 모두 값이 없으면 빈 배열
            if (branchFindDataSet.Count == 0)
            {
                branchArray = [ ];
                return;
            }

            // 바이트 종류가 하나이면 
            if (branchFindDataSet.Count == 1)
            {
                TreeNodeFindData branchFindData = branchFindDataSet[0];
                byte keyByte = branchFindData.keyByte;
                int nameCount = branchFindData.nameCount;

                // 하위 트리 생성
                TreeNode node = new TreeNode(descriptorArray, keyByte, 1, ref nodeFindDataSet);
                for (int i = 0; i < descriptorArray.Length; i++)
                {
                    EventDataDescriptor descriptor = descriptorArray[i];
                    string name = descriptor.Name;
                    if (name[0] == keyByte)
                    {
                        nodeFindDataSet.AddValue(descriptor);
                    }
                }

                branchArray = [ node ];
                return;
            }



            // 깊이 1 노드들 중 가장 많은 노드 수 찾기
            int maxBranchNodeCount = 0;
            for (int branchFindDataIndex = 0; branchFindDataIndex < branchFindDataSet.Count; branchFindDataIndex++)
            {
                ref TreeNodeFindData branchFindData = ref branchFindDataSet[branchFindDataIndex];
                int nameCount = branchFindData.nameCount;

                // 할당
                if (maxBranchNodeCount < nameCount)
                {
                    maxBranchNodeCount = nameCount;
                }
            }

            // 브랜치에서 공유할 설명자 세트 생성
            ArrayIndexSet<EventDataDescriptor> descriptorIndexSet = new ArrayIndexSet<EventDataDescriptor>(descriptorArray, maxBranchNodeCount);

            // 하위 노드 배열 생성
            branchArray = new TreeNode[branchFindDataSet.Count];
            for (int branchIndex = 0; branchIndex < branchArray.Length; branchIndex++)
            {
                ref TreeNodeFindData branchFindData = ref branchFindDataSet[branchIndex];

                // 설명자 세트 초기화
                descriptorIndexSet.Reset(branchFindData.nameCount);
                for (int descriptorIndex = 0; descriptorIndex < descriptorArray.Length; descriptorIndex++)
                {
                    EventDataDescriptor descriptor = descriptorArray[descriptorIndex];

                    // 시작 바이트가 키와 같으면 추가
                    if (descriptor.Name[0] == branchFindData.keyByte)
                    {
                        // 추가
                        descriptorIndexSet.Add(descriptorIndex);

                        // 전부 찾으면 break
                        if (branchFindData.nameCount == descriptorIndexSet.Count)
                            break;
                    }
                }

                // 공용 탐색 세트 초기화
                nodeFindDataSet.Reset(1);
                for (int i = 0; i < descriptorIndexSet.Count; i++)
                {
                    EventDataDescriptor descriptor = descriptorIndexSet[i];
                    if (descriptor.Name.Length <= 1) continue;

                    // 세트에 추가
                    TreeNodeFindData nodeFindData = new TreeNodeFindData((byte)descriptor.Name[1]);
                    nodeFindDataSet.Add(nodeFindData);
                }

                // 하위 트리 생성
                TreeNode node = new TreeNode(desciptorIndexSet, keyByte, 1, ref nodeFindDataSet);
                for (int i = 0; i < descriptorArray.Length; i++)
                {
                    EventDataDescriptor descriptor = descriptorArray[i];
                    string name = descriptor.Name;
                    if (name[0] == keyByte)
                    {
                        nodeFindDataSet.AddValue(descriptor);
                    }
                }

                for (int i = 0; i < nodeFindDataSet.Count; i++)
                {
                    ref TreeNodeFindData nodeFindData = ref nodeFindDataSet[i];
                    byte keyByte = nodeFindData.keyByte;
                    int nameCount2 = nodeFindData.nameCount;
                }
            }

            int branchCount;
            for (int i = 0; i < descriptorArray.Length; i++)
            {
                EventDataDescriptor descriptor = descriptorArray[i];
                string name = descriptor.Name;
            }
            this.branchArray = branchArray;


            // 하위 노드 탐색
            
        }
    }
    public class TreeNode
    {
        public byte startByte;
        public int keyStartIndex;
        public TreeNode[]? branchArray;
        public EventDataDescriptor? value;

        public TreeNode(byte startByte, int keyStartIndex, TreeNode[]? branchArray, EventDataDescriptor? descriptor)
        {
            this.startByte = startByte;
            this.keyStartIndex = keyStartIndex;
            this.branchArray = branchArray;
            this.value = descriptor;
        }
        public TreeNode(ArrayIndexSet<EventDataDescriptor> descriptorArrayIndexSet, byte startByte, int keyStartIndex, 
            ref TreeNodeFindData.Set nodeFindDataSet)
        {
            this.startByte = startByte;
            this.keyStartIndex = keyStartIndex;

            this.branchArray = FindBranch(descriptorArrayIndexSet, keyStartIndex + 1, 
                                          ref nodeFindDataSet, out value);
        }

        public static TreeNode? GetTreeNode(byte keyStartByte, int keyStartIndex, int offset,
            ArrayIndexSet<EventDataDescriptor> descriptorArrayIndexSet,
            ArrayIndexSet<EventDataDescriptor> 
            ref TreeNodeFindData.Set nodeFindDataSet)
        {
            // 단 한 개만 존재하는 경우
            if (descriptorArrayIndexSet.Count == 1)
            {
                EventDataDescriptor descriptor = descriptorArrayIndexSet[0];

                // 이름 길이 초과
                if (descriptor.Name.Length - 1 < keyStartIndex + offset)
                {
                    // null 반환
                    return null;
                }

                // 노드 생성 및 반환
                return new TreeNode(keyStartByte, keyStartIndex, null, descriptor);
            }

            // 노드 탐색 세트 초기화
            nodeFindDataSet.Reset(keyStartIndex + offset);
            for (int i = 0; i < descriptorArrayIndexSet.Count; i++)
            {
                EventDataDescriptor descriptor = descriptorArrayIndexSet[i];

                TreeNodeFindData nodeFindData = new TreeNodeFindData((byte)descriptor.Name[keyStartIndex]);
                nodeFindDataSet.Add(nodeFindData);
            }

            // 설명자 순회
            for (int i = 0; i < descriptorArrayIndexSet.Count; i++)
            {
                EventDataDescriptor descriptor = descriptorArrayIndexSet[i];
                string descriptorName = descriptor.Name;

                // 이름 길이 초과
                if (descriptorName.Length - 1 < keyStartIndex + offset) 
                {
                    .
                    continue;
                }

                // 바이트가 같으면
                if (descriptorName[keyStartIndex] == keyStartByte)
                {
                    // 하위 노드 생성
                    TreeNode node = new TreeNode(descriptorArrayIndexSet, keyStartByte, keyStartIndex + 1, ref nodeFindDataSet);
                    return node;
                }
            }
        }

        static TreeNode[]? FindBranch(ref ArrayIndexSet<EventDataDescriptor> descriptorArrayIndexSet, ref ArrayIndexSet<EventDataDescriptor> subDescriptorArrayIndexSet,
            int nameKeyFindIndex, 
            ref TreeNodeFindData.Set nodeFindDataSet, out EventDataDescriptor? endDescriptor)
        {
            // out 초기화
            endDescriptor = null;

            // 공용 탐색 세트 초기화
            nodeFindDataSet.Reset(nameKeyFindIndex);

            // 설명자 탐색
            for (int descriptorIndex = 0; descriptorIndex < descriptorArrayIndexSet.Count; descriptorIndex++)
            {
                EventDataDescriptor descriptor = descriptorArrayIndexSet[descriptorIndex];
                string name = descriptor.Name;

                // 이름을 끝까지 읽은 설명자를 트리에 값으로 등록
                if (endDescriptor == null)
                {
                    // 이름 끝까지 탐색함
                    if (nameKeyFindIndex >= name.Length) 
                    {
                        endDescriptor = descriptorArrayIndexSet[descriptorIndex];
                        continue;
                    }
                }

                byte keyByte = (byte)name[nameKeyFindIndex];

                // 이미 키가 존재하는 지 확인
                bool isKeyByteExist = false;
                for (int branchFindDataIndex = 0; branchFindDataIndex < nodeFindDataSet.count; branchFindDataIndex++)
                {
                    ref TreeNodeFindData branchFindData = ref nodeFindDataSet[branchFindDataIndex];

                    // 키가 존재하면 카운트 증가
                    if (branchFindData.keyByte == keyByte)
                    {
                        isKeyByteExist = true;
                        branchFindData.nameCount++;
                        break;
                    }
                }

                // 키가 존재하지 않으면 새 데이터 추가
                if (isKeyByteExist == false)
                {
                    TreeNodeFindData newBranchFindData = new TreeNodeFindData(keyByte);
                    nodeFindDataSet.Add(newBranchFindData);
                }
            }

            // 이번 키가 한 종류임
            if (nodeFindDataSet.Count == 1)
            {
                return FindBranch(descriptorArrayIndexSet, nameKeyFindIndex + 1, ref nodeFindDataSet);
            }

            // 모든 이름이 끝까지 탐색됨
            if (nodeFindDataSet.Count == 0)
            {
                return null;
            }

            // 이번 키가 여러 종류임
            ArrayIndexSet<EventDataDescriptor> branchNameIndexSet;
            TreeNode[] result = new TreeNode[nodeFindDataSet.Count];
            for (int i = 0; i < nodeFindDataSet.Count; i++)
            {
                ref TreeNodeFindData branchFindData = ref nodeFindDataSet[i];
                byte keyByte = branchFindData.keyByte;

                result[i] = new TreeNode(branchNameIndexSet, keyByte, nameKeyFindIndex + 1, 
                    ref nodeFindDataSet);
            }
        }
    }

    #endregion


    #region Static


    #endregion


    #region Instance

    // 필드

    public EventDataDescriptor[] descriptorArray;
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
        descriptorArray = descriptorEnumerable.ToArray();


        int descriptorIndex = 0;
        foreach (EventDataDescriptor descriptor in descriptorEnumerable)
        {
            nameArray[descriptorIndex++] = descriptor.Name;
        }

        treeRoot = CreateTreeNodeRoot(nameArray, 0, descriptorCount);

        /// <summary>
        /// 트리 노드 생성
        /// </summary>
        /// <return> 
        /// true & notnull : 하위 노드에서 다른 키 발견 <br/>
        /// true & null : 하위 노드에서 다른 키 발견되지 않음 <br/>
        /// false & null : 모든 이름이 끝까지 탐색됨
        /// </return>
        static bool TryGetTreeNodes(string[] nameArray, int byteIndex, int keyStartIndex, int maxNodeCount,
            out TreeNode[]? treeNodes)
        {
            TreeNodeDataSet nodeDataSet = new TreeNodeDataSet(stackalloc TreeNodeData[maxNodeCount]);

            bool isExist = false;

            Span<byte> characterSpan = stackalloc byte[maxNodeCount];
            for (int nameIndex = 0; nameIndex < nameArray.Length; nameIndex++)
            {
                string name = nameArray[nameIndex];
                
                // 길이 초과시 continue
                if (byteIndex >= name.Length) 
                    continue;

                byte keyByte = (byte)name[byteIndex];

                // 값이 있는 첫 번째 값이면
                if (isExist == false) 
                {
                    isExist = true;
                }


                bool isNewByte = true;                
                for (int nodeDataIndex = 0; nodeDataIndex < nodeDataSet.Count; nodeDataIndex++)
                {
                    ref TreeNodeData nodeData = ref nodeDataSet[nodeDataIndex];

                    if (nodeData.curKeyByte == keyByte)
                    {
                        nodeData.AddName(nameIndex);

                        isNewByte = false;
                        break;
                    }
                }
                if (isNewByte)
                {
                    // 새 노드 데이터 추가
                    TreeNodeData newNodeData = new TreeNodeData(keyByte, keyStartIndex);
                    nodeDataSet.Add(newNodeData);
                }
            }

            // 모두 값이 없으면 null 리턴
            if (isExist == false)
            {
                treeNodes = null;
                return false;
            }

            int curKeyIndex = keyStartIndex;

            for (int nodeDataIndex = 0; nodeDataIndex < nodeDataSet.Count; nodeDataIndex++)
            {
                ref TreeNodeData nodeData = ref nodeDataSet[nodeDataIndex];

                for (int nextByteIndex = byteIndex + 1; ; nextByteIndex++)
                {
                    TreeNode[]? children;

                    while (true)
                    {
                        if (TryGetTreeNodes(nameArray, nextByteIndex, nodeData.keyStartIndex, maxNodeCount, out children)) 
                        {
                            // 모든 키가 전부 같으면 -> 현재 키 길이 증가
                            if (children == null) 
                            {
                                curKeyIndex++;
                                continue;
                            }
                            // 다른 키가 존재하면
                            else
                            {
                                treeNodes = new TreeNode[nodeDataSet.Count];
                                for (int i = 0; i < nodeDataSet.Count; i++)
                                {
                                    ref TreeNodeData newNodeData = ref nodeDataSet[i];
                                    string? value = null;
                                    for (int childrenIndex = 0; childrenIndex < children.Length; childrenIndex++)
                                    {
                                        TreeNode child = children[childrenIndex];
                                        if (child.keyStartIndex == newNodeData.keyStartIndex)
                                        {
                                            value = child.value;
                                            break;
                                        }
                                    }
                                    treeNodes[i] = new TreeNode(newNodeData.keyStartByte, newNodeData.keyStartIndex, children, null);
                                }
                                return true;
                            }
                        }
                        else
                        {

                        }
                    }
                    
                    TreeNode[]? children = GetTreeNodes(nameArray, nextByteIndex, nodeData.keyStartIndex);
                    if (children != null) {
                        break;
                    }

                    nodeData.AddKeyLength();
                }
            }

        }
        
        static KeyByteDataSet GetKeyByteSet(ReadOnlySequence<string> nameSequence, int byteIndex)
        {
            int nameSequenceCount = (int)nameSequence.Length;
            KeyByteDataSet keyByteSet = new KeyByteSet(stackalloc byte[nameSequenceCount]);

            for (int nameIndex = 0; nameIndex < nameSequenceCount; nameIndex++)
            {
                string name = nameSequence[nameIndex];
                if (byteIndex >= name.Length) continue;
                byte keyByte = (byte)name[byteIndex];
                keyByteSet.Add(keyByte);
            }
            return keyByteSet;
        }
    }

    static TreeNodeRoot CreateTreeNodeRoot(EventDataDescriptor[] descriptorArray, string[] nameArray, int nameCount)
    {
        KeyByteDataSet keyByteDataSet = new KeyByteDataSet(nameArray, stackalloc KeyByteData[nameCount]);

        for (int nameIndex = 0; nameIndex < nameCount; nameIndex++)
        {
            string name = nameArray[nameIndex];

            if (name.Length == 0) 
                continue;

            byte keyByte = (byte)name[0];

            keyByteDataSet.Add(keyByte, nameIndex, nameCount);
        }
        KeyByteDataSet? childKeyByteSet;
        for ()
        TreeNode[] treeNodes = new TreeNode[keyByteDataSet.keyByteSetCount];
        for (int i = 0; i < keyByteDataSet.keyByteSetCount; i++)
        {
            ref KeyByteData keyByteData = ref keyByteDataSet[i];

            treeNodes[i] = new TreeNode(keyByteData.keyByte, 0, null, null);
        }
        return new TreeNodeRoot(treeNodes);
    }
    static void SetKeyByteDataSet(ref KeyByteDataSet keyByteDataSet, ArrayIndexSet<string> nameIndexSet, int byteIndex)
    {
        keyByteDataSet.Reset();

        int nameIndexSetCount = nameIndexSet.Count;
        for (int nameIndex = 0; nameIndex < nameIndexSetCount; nameIndex++)
        {
            string name = nameIndexSet[nameIndex];

            if (byteIndex >= name.Length) 
                continue;

            byte keyByte = (byte)name[byteIndex];

            keyByteDataSet.Add(keyByte, nameIndex, nameIndexSetCount);
        }
    }
    #endregion
}