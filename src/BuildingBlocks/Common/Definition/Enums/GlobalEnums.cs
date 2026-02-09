namespace Hello100Admin.BuildingBlocks.Common.Definition.Enums
{
    /// <summary>
    /// 관리자 구분
    /// </summary>
    public enum ManagerType
    {
        None = 0,

        /// <summary>
        /// 전체 관리자
        /// </summary>
        SuperManager = 1,

        /// <summary>
        /// 중간관리자
        /// </summary>
        Manager = 2,

        /// <summary>
        /// 병원 관리자
        /// </summary>
        HospitalManager = 4
    }

    /// <summary>
    /// DB
    /// </summary>
    public enum DataSource
    {
        /// <summary>
        /// 
        /// </summary>
        Hello100 = 0
    }

    /// <summary>
    /// 
    /// </summary>
    public enum AccountHospitalListSearchType
    {
        /// <summary>
        /// 1: 병원명 검색
        /// </summary>
        name = 0
    }

    /// <summary>
    /// 
    /// </summary>
    public enum HospitalListSearchType
    {
        /// <summary>
        /// 1: 병원명 검색
        /// </summary>
        name = 0,
        /// <summary>
        /// 2: 요양기관번호 검색
        /// </summary>
        hosp_no = 1
    }
}
