using System.Text.Json.Serialization;

namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public sealed record PostDoctorUntactWeeksReservationRequest
    {
        public required string HospNo { get; init; }
        public required string EmplNo { get; init; }
        public int WeekNum { get; init; }
        public int UntactRsrvIntervalTime { get; init; }
        [JsonIgnore]
        public int UntactRsrvIntervalCnt { get; init; } = 1;
        public string UntactAvaStartTime { get; init; }
        public string UntactAvaEndTime { get; init; }
        public string UntactAvaUseYn { get; init; }
        public List<PostDoctorUntactWeeksReservationRequestItem> EghisDoctRsrvDetailInfoList { get; init; }
    }

    public sealed record PostMyDoctorUntactWeeksReservationRequest
    {
        public required string EmplNo { get; init; }
        public int WeekNum { get; init; }
        public int UntactRsrvIntervalTime { get; init; }
        [JsonIgnore]
        public int UntactRsrvIntervalCnt { get; init; } = 1;
        public string UntactAvaStartTime { get; init; }
        public string UntactAvaEndTime { get; init; }
        public string UntactAvaUseYn { get; init; }
        public List<PostDoctorUntactWeeksReservationRequestItem> EghisDoctRsrvDetailInfoList { get; init; }
    }

    public sealed record PostDoctorUntactWeeksReservationRequestItem
    {
        public int RsIdx { get; init; }
        public int Ridx { get; init; }
        public string StartTime { get; init; }
        public string EndTime { get; init; }
        public int RsrvCnt { get; init; }
        public int ComCnt { get; init; }
        public string? RegDt { get; init; }
        public string ReceptType { get; init; }
    }
}
