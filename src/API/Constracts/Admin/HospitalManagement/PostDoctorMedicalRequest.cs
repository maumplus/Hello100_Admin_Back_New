namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class PostDoctorMedicalRequest
    {
        public required string HospNo { get; set; }
        public required string HospKey { get; set; }
        public required string EmplNo { get; set; }
        public List<string> MdCdList { get; set; }
    }

    public class PostMyDoctorMedicalRequest
    {
        public required string EmplNo { get; set; }
        public List<string> MdCdList { get; set; }
    }
}
