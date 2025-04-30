namespace Johwa.Common.Collection;

public partial class ReadOnlyByteSpanTree<TValue>
{
    #region Static

    static ReadOnlyByteSpanTree<TNewValue>.Node CreateNewTreeNode<TNewValue>(Node originalNode, Func<TValue, TNewValue> valueSelector)
    {
        return new ReadOnlyByteSpanTree<TNewValue>.Node(
            originalNode.nodeIndex,
            originalNode.keySlice,
            CreateNewTreeNodeValueInfo(originalNode, valueSelector),
            CreateNewTreeNodeChildNodeArray(originalNode, valueSelector),
            originalNode.branchDepth
        );
    }
    static ReadOnlyByteSpanTree<TNewValue>.ValueInfo? CreateNewTreeNodeValueInfo<TNewValue>(Node originalNode, Func<TValue, TNewValue> valueSelector)
    {
        if (originalNode.valueInfo == null) 
        {
            return null;
        }
        else
        {
            return new ReadOnlyByteSpanTree<TNewValue>.ValueInfo(
                valueSelector.Invoke(originalNode.valueInfo.value),
                originalNode.valueInfo.valueIndex
            );
        }
    }
    static ReadOnlyByteSpanTree<TNewValue>.Node[]? CreateNewTreeNodeChildNodeArray<TNewValue>(Node originalNode, Func<TValue, TNewValue> valueSelector)
    {
        if (originalNode.childNodeArray == null)
        {
            return null;
        }
        else
        {
            ReadOnlyByteSpanTree<TNewValue>.Node[] newTreeNodeChildNodeArray 
                = new ReadOnlyByteSpanTree<TNewValue>.Node[originalNode.childNodeArray.Length];
                
            for (int i = 0; i < newTreeNodeChildNodeArray.Length; i++)
            {
                Node originalNodeChild = originalNode.childNodeArray[i];
                newTreeNodeChildNodeArray[i] = CreateNewTreeNode(originalNodeChild, valueSelector);
            }

            return newTreeNodeChildNodeArray;
        }
    }

    #endregion


    #region Instance

    public ReadOnlyByteSpanTree<TNewValue> CreateNewTree<TNewValue>(Func<TValue, TNewValue> valueSelector)
    {
        return new ReadOnlyByteSpanTree<TNewValue>(
            CreateNewTreeNode(rootNode, valueSelector), 
            valueCount, 
            maxNodeDepth,
            maxBranchDepth
        );
    }
    
    #endregion
}