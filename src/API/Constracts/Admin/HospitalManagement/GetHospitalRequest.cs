namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class GetHospitalRequest
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public required string HospNo { get; init; }
    }
}
