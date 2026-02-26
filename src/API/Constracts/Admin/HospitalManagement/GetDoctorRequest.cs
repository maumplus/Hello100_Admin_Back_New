namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class GetDoctorRequest
    {
        public required string HospNo { get; set; }
        public required string EmplNo { get; set; }
    }
}
