namespace Johwa.Resources.ApplicationRoleConnectionMetadata;

/// <summary>
/// 애플리케이션 역할 연결 메타데이터 타입 <br/>
/// Type of metadata value
/// </summary>
public enum ApplicationRoleConnectionMetadataType
{
    /// <summary>
    /// 메타데이터 값이 설정된 값 이하인지 비교 <br/>
    /// the metadata value (integer) is less than or equal to the guild's configured value (integer)
    /// </summary>
    IntegerLessThanOrEqual = 1,

    /// <summary>
    /// 메타데이터 값이 설정된 값 이상인지 비교 <br/>
    /// the metadata value (integer) is greater than or equal to the guild's configured value (integer)
    /// </summary>
    IntegerGreaterThanOrEqual = 2,

    /// <summary>
    /// 메타데이터 값이 설정된 값과 같은지 비교 <br/>
    /// the metadata value (integer) is equal to the guild's configured value (integer)
    /// </summary>
    IntegerEqual = 3,

    /// <summary>
    /// 메타데이터 값이 설정된 값과 같지 않은지 비교 <br/>
    /// the metadata value (integer) is not equal to the guild's configured value (integer)
    /// </summary>
    IntegerNotEqual = 4,

    /// <summary>
    /// 메타데이터 시간이 설정된 날짜 이전인지 비교 <br/>
    /// the metadata value (ISO8601 string) is less than or equal to the guild's configured value (integer; days before current date)
    /// </summary>
    DatetimeLessThanOrEqual = 5,

    /// <summary>
    /// 메타데이터 시간이 설정된 날짜 이후인지 비교 <br/>
    /// the metadata value (ISO8601 string) is greater than or equal to the guild's configured value (integer; days before current date)
    /// </summary>
    DatetimeGreaterThanOrEqual = 6,

    /// <summary>
    /// 메타데이터 불리언 값이 설정값과 같은지 비교 <br/>
    /// the metadata value (integer) is equal to the guild's configured value (integer; 1)
    /// </summary>
    BooleanEqual = 7,

    /// <summary>
    /// 메타데이터 불리언 값이 설정값과 같지 않은지 비교 <br/>
    /// the metadata value (integer) is not equal to the guild's configured value (integer; 1)
    /// </summary>
    BooleanNotEqual = 8
}