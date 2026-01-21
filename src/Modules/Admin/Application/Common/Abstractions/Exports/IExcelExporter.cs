namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Exports
{
    public interface IExcelExporter
    {
        byte[] Export<T>(IReadOnlyList<T> rows, string sheetName, IReadOnlyList<ExcelColumn<T>> columns);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rows"></param>
        /// <param name="sheetName"></param>
        /// <param name="title">제목</param>
        /// <param name="columns"></param>
        /// <param name="titleFontSize">제목 글자 크기</param>
        /// <param name="titleBold">제목 Bold 여부</param>
        /// <returns></returns>
        public byte[] Export<T>(
            IReadOnlyList<T> rows,
            string sheetName,
            string title,
            IReadOnlyList<ExcelColumn<T>> columns,
            int titleFontSize = 18,
            bool titleBold = true);
    }
}
