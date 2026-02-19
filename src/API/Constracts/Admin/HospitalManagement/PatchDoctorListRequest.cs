namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class PatchDoctorInfo
    {
        public string EmplNo { get; set; }
        public int FrontViewRole { get; set; }
    }

    public class PatchDoctorListRequest
    {
        public string HospNo { get; set; }
        public List<PatchDoctorInfo> DoctorList { get; set; }
    }
}
