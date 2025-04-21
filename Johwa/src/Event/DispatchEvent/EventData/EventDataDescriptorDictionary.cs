using System.Buffers;
using Johwa.Common.Collection;

namespace Johwa.Event.Data;

public class EventDataDescriptorDictionary
{
    #region Object

    public struct TreeNodeFindData
    {
        public ref struct Set
        {
            public Span<TreeNodeFindData> branchFindDataSpan;
            int byteTypeCount;
            public int ByteTypeCount => byteTypeCount;
            public readonly int nameKeyFindIndex;

            public Set(Span<TreeNodeFindData> branchFindDataSpan, int nameKeyFindIndex)
            {
                this.branchFindDataSpan = branchFindDataSpan;
                this.byteTypeCount = 0;
                this.nameKeyFindIndex = nameKeyFindIndex;
            }
            public Set ExtractSet(Span<TreeNodeFindData> newFindDataSpan, int nameKeyFindIndex)
            {
                if (newFindDataSpan.Length < byteTypeCount)
                    throw new ArgumentOutOfRangeException("브랜치 데이터가 너무 많습니다.");

                Set set = new Set(newFindDataSpan, nameKeyFindIndex);
                for (int i = 0; i < byteTypeCount; i++)
                {
                    TreeNodeFindData branchFindData = branchFindDataSpan[i];
                    set.branchFindDataSpan[i] = branchFindData;
                }

                return set;
            }
            public void Reset(int nameKeyFindIndex)
            {
                this.nameKeyFindIndex = nameKeyFindIndex;
                this.byteTypeCount = 0;
            }
            public ref TreeNodeFindData this[int index]
                => ref Get(index);
            public ref TreeNodeFindData Get(int index)
            {
                if (index < 0 || index >= byteTypeCount)
                    throw new ArgumentOutOfRangeException("인덱스가 범위를 벗어났습니다.");

                return ref branchFindDataSpan[index];
            }
            public void Add(TreeNodeFindData branchFindData)
            {
                if (byteTypeCount >= branchFindDataSpan.Length)
                    throw new ArgumentOutOfRangeException("브랜치 데이터가 너무 많습니다.");

                branchFindDataSpan[byteTypeCount++] = branchFindData;
            }
            public void AddValue(EventDataDescriptor descriptor)
            {
                if (byteTypeCount >= branchFindDataSpan.Length)
                    throw new ArgumentOutOfRangeException("브랜치 데이터가 너무 많습니다.");

                if (descriptor.Name.Length == 0) 
                    throw new ArgumentException("이름 길이가 0입니다.");

                for (int i = 0; i < byteTypeCount; i++)
                {
                    ref TreeNodeFindData branchFindData = ref branchFindDataSpan[i];
                    if (branchFindData.keyByte == (byte)descriptor.Name[0])
                    {
                        branchFindData.nameCount++;
                        return;
                    }
                }
                Add(new TreeNodeFindData((byte)descriptor.Name[0], 1));
            }
        }

        public byte keyByte;
        public int nameCount;

        public TreeNodeFindData(byte keyByte, int nameCount)
        {
            this.keyByte = keyByte;
            this.nameCount = nameCount;
        }
    }
    public class TreeNodeRoot
    {
        public TreeNode[]? branchArray;
        public EventDataDescriptor? value;

        public TreeNodeRoot(EventDataDescriptor[] descriptorArray, int keyByteTypeCount)
        {
            // 비었음
            if (descriptorArray.Length == 0)
            {
                branchArray = null;
                value = null;
                return;
            }

            // 단 한 개만 존재하는 경우
            if (descriptorArray.Length == 1)
            {
                EventDataDescriptor descriptor = descriptorArray[0];
                string name = descriptor.Name;

                // 이름 길이 0
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
            TreeNodeFindData.Set sharedNodeFindDataSet = new (stackalloc TreeNodeFindData[maxBranchCount], 0);


            // 설명자 탐색
            for (int i = 0; i < descriptorArray.Length; i++)
            {
                EventDataDescriptor descriptor = descriptorArray[i];
                if (descriptor.Name.Length == 0) continue;

                // 브랜치 탐색 데이터 생성
                TreeNodeFindData branchFindData = new TreeNodeFindData((byte)descriptor.Name[0]);

                sharedNodeFindDataSet.Add(branchFindData);
            }

            // 탐색 결과 추출
            TreeNodeFindData.Set branchFindDataSet = sharedNodeFindDataSet.ExtractSet(stackalloc TreeNodeFindData[sharedNodeFindDataSet.byteTypeCount], 0);

            // 모두 값이 없으면 모두 null
            if (branchFindDataSet.ByteTypeCount == 0)
            {
                branchArray = null;
                value = null;
                return;
            }

            // 바이트 종류가 하나이면 
            if (branchFindDataSet.ByteTypeCount == 1)
            {
                TreeNodeFindData branchFindData = branchFindDataSet[0];
                byte keyByte = branchFindData.keyByte;
                int nameCount = branchFindData.nameCount;

                // 하위 트리 생성
                TreeNode node = new TreeNode(descriptorArray, keyByte, 1, ref sharedNodeFindDataSet);
                for (int i = 0; i < descriptorArray.Length; i++)
                {
                    EventDataDescriptor descriptor = descriptorArray[i];
                    string name = descriptor.Name;
                    if (name[0] == keyByte)
                    {
                        sharedNodeFindDataSet.AddValue(descriptor);
                    }
                }

                branchArray = [ node ];
                return;
            }



            // 깊이 1 노드들 중 가장 많은 노드 수 찾기
            int maxBranchNodeCount = 0;
            for (int branchFindDataIndex = 0; branchFindDataIndex < branchFindDataSet.ByteTypeCount; branchFindDataIndex++)
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
            branchArray = new TreeNode[branchFindDataSet.ByteTypeCount];
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
                sharedNodeFindDataSet.Reset(1);
                for (int i = 0; i < descriptorIndexSet.Count; i++)
                {
                    EventDataDescriptor descriptor = descriptorIndexSet[i];
                    if (descriptor.Name.Length <= 1) continue;

                    // 세트에 추가
                    TreeNodeFindData nodeFindData = new TreeNodeFindData((byte)descriptor.Name[1]);
                    sharedNodeFindDataSet.Add(nodeFindData);
                }

                // 하위 트리 생성
                TreeNode node = new TreeNode(desciptorIndexSet, keyByte, 1, ref sharedNodeFindDataSet);
                for (int i = 0; i < descriptorArray.Length; i++)
                {
                    EventDataDescriptor descriptor = descriptorArray[i];
                    string name = descriptor.Name;
                    if (name[0] == keyByte)
                    {
                        sharedNodeFindDataSet.AddValue(descriptor);
                    }
                }

                for (int i = 0; i < sharedNodeFindDataSet.ByteTypeCount; i++)
                {
                    ref TreeNodeFindData nodeFindData = ref sharedNodeFindDataSet[i];
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
        public byte keyStartByte;
        public int keyStartIndex;
        public TreeNode[]? branchArray;
        public EventDataDescriptor? value;

        public TreeNode(byte startByte, int keyStartIndex, TreeNode[]? branchArray, EventDataDescriptor? descriptor)
        {
            this.keyStartByte = startByte;
            this.keyStartIndex = keyStartIndex;
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
            keyStartIndex = -1;
            branchArray = null;
            value = descriptor;
        }

        public static TreeNode? GetRoot(EventDataDescriptor[] descriptorArray)
        {
            // 비었음 -> null
            if (descriptorArray.Length == 0) {
                return null;
            }

            // 단 한 개만 존재하는 경우
            if (descriptorArray.Length == 1)
            {
                EventDataDescriptor descriptor = descriptorArray[0];
                string name = descriptor.Name;

                // 이름 길이 0
                if (name.Length == 0)
                    throw new ArgumentException("이름 길이가 0입니다.");

                return new TreeNode(descriptor);
            }

            // 깊이 1 노드 탐색 세트
            TreeNodeFindData.Set depth1FindDataSet = GetNodeFindDataSet(
                descriptorArray, 
                stackalloc TreeNodeFindData[GetByteTypeCount(descriptorArray, 0, Math.Min(keyByteTypeCount, descriptorArray.Length))], 
                0
            );

            // 모두 길이가 0 -> null
            if (depth1FindDataSet.ByteTypeCount == 0) {
                return null;
            }

            // 바이트 종류가 하나이면 
            if (depth1FindDataSet.ByteTypeCount == 1)
            {
                TreeNodeFindData depth1FindData = depth1FindDataSet[0];
                byte depth1FindDataKeyByte = depth1FindData.keyByte;
                int depth1FindDataNameCount = depth1FindData.nameCount;

                return GetTreeNode(
                    descriptorArray,
                    1,
                    depth1FindDataKeyByte
                );
            }

            // 브랜치에서 공유할 설명자 세트 생성
            ArrayIndexSet<EventDataDescriptor> descriptorIndexSet = new ArrayIndexSet<EventDataDescriptor>(descriptorArray, maxBranchNodeCount);

            // 하위 노드 배열 생성
            branchArray = new TreeNode[depth1FindDataSet.ByteTypeCount];
            for (int branchIndex = 0; branchIndex < branchArray.Length; branchIndex++)
            {
                ref TreeNodeFindData branchFindData = ref depth1FindDataSet[branchIndex];

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

                for (int i = 0; i < nodeFindDataSet.ByteTypeCount; i++)
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
        }

        /// <summary>
        ///  name[KeyStartIndex + Offset]을 읽습니다
        /// </summary>
        /// <param name="keyStartByte"></param>
        /// <param name="keyIndex"></param>
        /// <param name="offset"></param>
        /// <param name="descriptorArrayIndexSet"></param>
        /// <param name="childNodeDescriptorArrayIndexSet"></param>
        /// <param name="nodeFindDataSet"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static TreeNode? GetTreeNode(EventDataDescriptor[] originalDescriptorArray, int keyIndex, byte keyStartByte)
        {
            ArrayIndexSpan<EventDataDescriptor> descriptorIndexSpan
                = SplitToArrayIndexSpan(
                    originalDescriptorArray, 
                    stackalloc int[GetKeyByteCount(originalDescriptorArray, keyIndex, keyStartByte)], 
                    keyIndex, 
                    keyStartByte
                );
            
            // 단 한 개만 존재하는 경우
            if (descriptorIndexSpan.Count == 1)
            {
                EventDataDescriptor descriptor = descriptorIndexSpan[0];

                // 이름 길이 초과
                if (keyIndex >= descriptor.Name.Length)
                {
                    // null 반환
                    return null;
                }

                // 노드 생성 및 반환
                return new TreeNode(descriptor);
            }


            // 노드 탐색 세트 초기화
            TreeNodeFindData.Set nodeFindDataSet = GetNodeFindDataSet(
                descriptorIndexSpan, 
                stackalloc TreeNodeFindData[GetByteTypeCount(descriptorIndexSpan, 0, descriptorIndexSpan.Count)], 
                keyIndex
            );

            TreeNode[] branchArray = new TreeNode[nodeFindDataSet.ByteTypeCount];
            for (int i = 0; i < nodeFindDataSet.ByteTypeCount; i++)
            {
                ref TreeNodeFindData nodeFindData = ref nodeFindDataSet[i];
                byte nodeFindDataKeyByte = nodeFindData.keyByte;

                branchArray[i] = GetTreeNode(
                    descriptorIndexSpan, 
                    keyIndex + 1, 
                    nodeFindDataKeyByte
                );
            }

            return new TreeNode(keyStartByte, keyIndex, branchArray, null);
        }
    
        static int GetByteTypeCount(ArrayIndexSpan<EventDataDescriptor> descriptorIndexSpan, int findDataIndex, int maxCount)
        {
            TreeNodeFindData.Set nodeFindDataSet = new (stackalloc TreeNodeFindData[maxCount], findDataIndex);

            for (int i = 0; i < descriptorIndexSpan.Count; i++)
            {
                EventDataDescriptor descriptor = descriptorIndexSpan[i];
                nodeFindDataSet.AddValue(descriptor);
            }
            return nodeFindDataSet.ByteTypeCount;
        }
        static int GetByteTypeCount(IList<EventDataDescriptor> descriptorList, int findDataIndex, int maxCount)
        {
            TreeNodeFindData.Set nodeFindDataSet = new (stackalloc TreeNodeFindData[maxCount], findDataIndex);

            for (int i = 0; i < descriptorList.Count; i++)
            {
                EventDataDescriptor descriptor = descriptorList[i];
                nodeFindDataSet.AddValue(descriptor);
            }
            return nodeFindDataSet.ByteTypeCount;
        }
        static TreeNodeFindData.Set GetNodeFindDataSet(ArrayIndexSpan<EventDataDescriptor> descriptorIndexSpan, Span<TreeNodeFindData> nodeFindDataSet, int findDataIndex)
        {
            TreeNodeFindData.Set result = new (nodeFindDataSet, findDataIndex);
            for (int i = 0; i < descriptorIndexSpan.Count; i++)
            {
                EventDataDescriptor descriptor = descriptorIndexSpan[i];
                result.AddValue(descriptor);
            }
            return result;
        }
        static TreeNodeFindData.Set GetNodeFindDataSet(IList<EventDataDescriptor> descriptorList, Span<TreeNodeFindData> nodeFindDataSet, int findDataIndex)
        {
            TreeNodeFindData.Set result = new (nodeFindDataSet, findDataIndex);
            for (int i = 0; i < descriptorList.Count; i++)
            {
                EventDataDescriptor descriptor = descriptorList[i];
                result.AddValue(descriptor);
            }
            return result;
        }
        
        /// <summary>
        /// 설명자 리스트의 설명자 이름의 특정 인덱스의 값과 지정한 바이트가 같은 경우의 수를 구합니다. 
        /// </summary>
        /// <param name="descriptorList"></param>
        /// <param name="keyIndex"></param>
        /// <param name="targetByte"></param>
        /// <returns></returns>
        static int GetKeyByteCount(IList<EventDataDescriptor> descriptorList, int keyIndex, byte targetByte)
        {
            int result = 0;
            for (int i = 0; i < descriptorList.Count; i++)
            {
                EventDataDescriptor descriptor = descriptorList[i];
                string descriptorName = descriptor.Name;

                // 이름 길이 초과
                if (keyIndex >= descriptorName.Length) continue;

                byte keyByte = (byte)descriptorName[keyIndex];
                
                // 바이트가 같으면
                if (keyByte == targetByte) 
                {
                    result++;
                    continue;
                }
            }
            return result;
        }

        static int GetKeyByteCount(ArrayIndexSpan<EventDataDescriptor> descriptorIndexSpan, int keyIndex, byte targetByte)
        {
            int result = 0;
            for (int i = 0; i < descriptorIndexSpan.Count; i++)
            {
                EventDataDescriptor descriptor = descriptorIndexSpan[i];
                string descriptorName = descriptor.Name;

                // 이름 길이 초과
                if (keyIndex >= descriptorName.Length) continue;

                byte keyByte = (byte)descriptorName[keyIndex];
                
                // 바이트가 같으면
                if (keyByte == targetByte) 
                {
                    result++;
                    continue;
                }
            }
            return result;
        }
        /// <summary>
        /// 설명자 리스트의 설명자 이름의 특정 인덱스의 값과 지정한 바이트가 같은 경우를 ArrayIndexSet으로 분리합니다.
        /// </summary>
        /// <param name="descriptorArray"></param>
        /// <param name="keyIndex"></param>
        /// <param name="keyStartByte"></param>
        /// <returns></returns>
        static ArrayIndexSet<EventDataDescriptor> SplitToArrayIndexSet(EventDataDescriptor[] descriptorArray, int keyIndex, byte keyStartByte)
        {
            ArrayIndexSet<EventDataDescriptor> result = new (
                descriptorArray, 
                GetKeyByteCount(descriptorArray, keyIndex, keyStartByte)
            );

            for (int i = 0; i < descriptorArray.Length; i++)
            {
                EventDataDescriptor descriptor = descriptorArray[i];
                string descriptorName = descriptor.Name;

                // 이름 길이 초과
                if (keyIndex >= descriptorName.Length) continue;

                // 바이트가 같으면
                if (descriptorName[keyIndex] == keyStartByte)
                {
                    // 자식 노드 설명자 인덱스 배열에 추가
                    result.Add(i);
                }
            }

            return result;
        }
        
        /// <summary>
        /// 설명자 리스트의 설명자 이름의 특정 인덱스의 값과 지정한 바이트가 같은 경우를 ArrayIndexSet으로 분리합니다.
        /// </summary>
        /// <param name="descriptorArray"></param>
        /// <param name="keyIndex"></param>
        /// <param name="keyStartByte"></param>
        /// <returns></returns>
        static ArrayIndexSet<EventDataDescriptor> SplitToArrayIndexSet(ArrayIndexSet<EventDataDescriptor> descriptorIndexSet, int keyIndex, byte keyStartByte)
        {
            ArrayIndexSet<EventDataDescriptor> result = new (
                descriptorIndexSet.originalValueList, 
                GetKeyByteCount(descriptorIndexSet, keyIndex, keyStartByte)
            );

            for (int i = 0; i < descriptorIndexSet.Count; i++)
            {
                int descriptorIndex = descriptorIndexSet.GetIndex(i);
                
                EventDataDescriptor descriptor = descriptorIndexSet.originalValueList[descriptorIndex];
                string descriptorName = descriptor.Name;

                // 이름 길이 초과
                if (keyIndex >= descriptorName.Length) continue;

                // 바이트가 같으면
                if (descriptorName[keyIndex] == keyStartByte)
                {
                    // 자식 노드 설명자 인덱스 배열에 추가
                    result.Add(descriptorIndex);
                }
            }

            return result;
        }
    
        static ArrayIndexSpan<EventDataDescriptor> SplitToArrayIndexSpan(EventDataDescriptor[] descriptorArray, Span<int> indexBuffer, int keyIndex, byte keyStartByte)
        {
            ArrayIndexSpan<EventDataDescriptor> result = new (
                descriptorArray, 
                indexBuffer
            );

            for (int i = 0; i < descriptorArray.Length; i++)
            {
                EventDataDescriptor descriptor = descriptorArray[i];
                string descriptorName = descriptor.Name;

                // 이름 길이 초과
                if (keyIndex >= descriptorName.Length) continue;

                // 바이트가 같으면
                if (descriptorName[keyIndex] == keyStartByte)
                {
                    // 자식 노드 설명자 인덱스 배열에 추가
                    result.Add(i);
                }
            }

            return result;
        }
        static ArrayIndexSpan<EventDataDescriptor> SplitToArrayIndexSpan(ArrayIndexSpan<EventDataDescriptor> descriptorIndexSpan, Span<int> indexBuffer, int keyIndex, byte keyStartByte)
        {
            ArrayIndexSpan<EventDataDescriptor> result = new (
                descriptorIndexSpan.originalValueSpan, 
                indexBuffer
            );

            for (int i = 0; i < descriptorIndexSpan.Count; i++)
            {
                int descriptorIndex = descriptorIndexSpan.GetIndex(i);
                
                EventDataDescriptor descriptor = descriptorIndexSpan.originalValueList[descriptorIndex];
                string descriptorName = descriptor.Name;

                // 이름 길이 초과
                if (keyIndex >= descriptorName.Length) continue;

                // 바이트가 같으면
                if (descriptorName[keyIndex] == keyStartByte)
                {
                    // 자식 노드 설명자 인덱스 배열에 추가
                    result.Add(descriptorIndex);
                }
            }

            return result;
        }
    }

    #endregion


    #region Static

    public const int keyByteTypeCount = ('Z' - 'A' + 1) + 1;

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