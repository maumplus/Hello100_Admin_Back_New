namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results
{
    public sealed class GetHospitalsUsingHello100ServiceResult
    {
        public long RowNum { get; set; }
        public string HospKey { get; set; } = default!;
        public string HospNo { get; set; } = default!;
        public string Name { get; set; } = default!;
        //public int DeskCnt { get; set; }
        //public int KioskCnt { get; set; }
    }
}
