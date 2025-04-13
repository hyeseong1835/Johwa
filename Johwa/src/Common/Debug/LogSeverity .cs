public enum LogSeverity
{
    /// <summary>
    ///     Debug level logging. This is the lowest level of logging and is used for detailed debugging information.
    /// </summary>
    Debug = 0,

    /// <summary>
    ///     Information level logging. This is used for general information messages that are not errors.
    /// </summary>
    Info = 1,

    /// <summary>
    ///     Warning level logging. This is used for warning messages that indicate a potential problem.
    /// </summary>
    Warning = 2,

    /// <summary>
    ///     Error level logging. This is used for error messages that indicate a failure.
    /// </summary>
    Error = 3,

    /// <summary>
    ///     Critical level logging. This is used for critical error messages that indicate a serious failure.
    /// </summary>
    Critical = 4,
}