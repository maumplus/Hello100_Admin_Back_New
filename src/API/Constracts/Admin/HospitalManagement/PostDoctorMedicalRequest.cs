namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class PostDoctorMedicalRequest
    {
        public string EmplNo { get; set; }
        public List<string> MdCdList { get; set; }
    }
}
