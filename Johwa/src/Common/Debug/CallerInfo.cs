namespace Johwa.Common.Debug;

public struct CallerInfo
{
    public string memberName;
    public string filePath;
    public int lineNumber;

    public CallerInfo(string memberName, string filePath, int lineNumber)
    {
        this.memberName = memberName;
        this.filePath = filePath;
        this.lineNumber = lineNumber;
    }
}