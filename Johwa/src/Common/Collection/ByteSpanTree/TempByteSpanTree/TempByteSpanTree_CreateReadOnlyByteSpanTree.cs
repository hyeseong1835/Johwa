using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Johwa.Common.Collection;

/// <summary>
/// 내부는 참조 변수로 이루어져 있기 때문에 복사하여도 정보를 공유함
/// </summary>
public partial struct TempByteSpanTree<TValue>
    where TValue : unmanaged
{
    #region Object
    
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

    #region 메서드

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

    #endregion

    #endregion
}