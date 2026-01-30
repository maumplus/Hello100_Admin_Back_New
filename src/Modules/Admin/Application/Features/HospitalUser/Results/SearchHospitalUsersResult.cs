using System.Text.Json.Serialization;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Results
{
    public class SearchHospitalUsersResult
    {
        public int RowNum { get; set; }
        public string UId { get; set; } = default!;
        public int MId { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string LoginTypeName { get; set; } = default!;
        public int Said { get; set; }
        public string RegDtView { get; set; } = default!;

        [JsonIgnore]
        public string SnsId { get; set; } = default!;
        [JsonIgnore]
        public string LoginType { get; set; } = default!;
        [JsonIgnore]
        public string RegDt { get; set; } = default!;
        [JsonIgnore]
        public string? AuthDt { get; set; }
    }
}
