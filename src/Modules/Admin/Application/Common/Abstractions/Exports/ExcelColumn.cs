using ClosedXML.Excel;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Exports
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Header">헤더명</param>
    /// <param name="Value">값</param>
    /// <param name="DataType">강제 타입</param>
    /// <param name="Format">숫자/날짜 포맷</param>
    /// <param name="Width">컬럼 폭 고정</param>
    /// <param name="Align">정렬</param>
    public sealed record ExcelColumn<T>(
        string Header,
        Func<T, object?> Value,
        XLDataType? DataType = null,
        string? Format = null,
        double Width = 10,
        XLAlignmentHorizontalValues Align = XLAlignmentHorizontalValues.Center
    );
}
