namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class PatchDoctorInfo
    {
        public string EmplNo { get; set; }
        public int FrontViewRole { get; set; }
    }

    public class PatchDoctorListRequest
    {
        public required string HospNo { get; set; }
        public List<PatchDoctorInfo> DoctorList { get; set; }
    }

    public class PatchMyDoctorListRequest
    {
        public List<PatchDoctorInfo> DoctorList { get; set; }
    }
}
