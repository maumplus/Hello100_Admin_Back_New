using Hello100Admin.Modules.Admin.Application.Common.Models;
using System.Text.Json.Serialization;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results
{
    public sealed class GetDeviceSettingResult<T>
    {
        public string Name { get; set; }
        public List<DeviceInfo> EmplList { get; set; }
        public DeviceRo<T> DeviceData { get; set; }
        public ListResult<DoctorBaseRo> DocList { get; set; }
    }

    public class DeviceInfo
    {
        public string DeviceNm { get; set; }
        public string EmplNo { get; set; }
    }

    public class DeviceRo<T>
    {
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
        public string DeviceNm { get; set; }
        public string HospNm { get; set; }
        public string DeviceType { get; set; }
        [JsonIgnore]
        public string HospKey { get; set; }
        public string InfoTxt { get; set; }
        [JsonIgnore]
        public string SetJsonStr { get; set; }
        public string UseYn { get; set; }
        public T SetJson { get; set; }
    }

    public class DoctorBaseRo
    {
        public int RowNum { get; set; } = 0;
        public string DoctNoDesc { get; set; }
        public string HospNo { get; set; }
        public string HospKey { get; set; }
        public string EmplNo { get; set; }
        public string ClinicYmd { get; set; }
        public string DoctNo { get; set; }
        public string DoctNm { get; set; }
        public string DeptCd { get; set; }
        public string DeptNm { get; set; }
        public int WeekNum { get; set; }
        public string UseYn { get; set; }
        public string WeeksNum { get; set; }
        public string WeeksNm { get; set; }
        public int RsrvCnt { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public int BreakStartHour { get; set; }
        public int BreakStartMinute { get; set; }
        public int BreakEndHour { get; set; }
        public int BreakEndMinute { get; set; }
        public int IntervalTime { get; set; }
        public string Message { get; set; }
        public int Hello100Role { get; set; }
        public int ViewRole { get; set; }
        public string ViewMinTime { get; set; }
        public string ViewMinCnt { get; set; }
        public string RegDt { get; set; }
        public int UntactStartHour { get; set; }
        public int UntactStartMinute { get; set; }
        public int UntactEndHour { get; set; }
        public int UntactEndMinute { get; set; }
        public int UntactIntervalTime { get; set; }
        public string UntactUseYn { get; set; }
        public int UntactBreakStartHour { get; set; }
        public int UntactBreakStartMinute { get; set; }
        public int UntactBreakEndHour { get; set; }
        public int UntactBreakEndMinute { get; set; }
        public string UntactJoinYn { get; set; }
        public int UntactRsrvCnt { get; set; }
        public string DoctModifyYn { get; set; }
        public string DoctFilePath { get; set; }
        public int FrontViewRole { get; set; }
    }

    public class TabletRo
    {
        [JsonIgnore]
        public string PayYn { get; set; } = "N";
        public string AddrYn { get; set; } = "N";
        public string DeptYn { get; set; } = "N";
        public string DeptBreakYn { get; set; } = "N";
        public string DetailYn { get; set; } = "N";
        public string ReceptYn { get; set; } = "N";
        [JsonIgnore]
        public string SimplePayYn { get; set; } = "N";
        public string WaitTimeYn { get; set; } = "N";
        public string ViewMinTime { get; set; } = "";
        public string NewReceiveYn { get; set; } = "N";
        [JsonIgnore]
        public string PrtBarcodeYn { get; set; } = "N";
        /// <summary>
        /// 환자 입력 유형 [0: 없음, 1: 휴대폰번호, 2: 주민등록번호, 3: 접수증바코드]
        /// </summary>
        public int PtntInputType { get; set; } = 1; // PtntType.TelNo;
        public string ReceiveMainSelect { get; set; } = "D";
        public string PopupYn { get; set; } = "Y";
        public string DefaultDeptCD { get; set; } = "";
        public string DefaultEmplNo { get; set; } = "";
        public string receiptState { get; set; } = "W";
        public string QrReceiptYn { get; set; } = "N";
        public string PurposeYn { get; set; } = "Y";
    }

    public class KioskRo
    {
        public string PayYn { get; set; } = "N";
        public string AddrYn { get; set; } = "N";
        public string DeptYn { get; set; } = "N";
        public string DeptBreakYn { get; set; } = "N";
        public string DetailYn { get; set; } = "N";
        public string ReceptYn { get; set; } = "N";
        public string SimplePayYn { get; set; } = "N";
        public string WaitTimeYn { get; set; } = "N";
        public string ViewMinTime { get; set; } = "";
        public string NewReceiveYn { get; set; } = "N";
        public string PrtBarcodeYn { get; set; } = "N";
        /// <summary>
        /// 환자 입력 유형 [0: 없음, 1: 휴대폰번호, 2: 주민등록번호, 3: 접수증바코드]
        /// </summary>
        public int PtntInputType { get; set; } = 1; // PtntType.TelNo;
        public string ReceiveMainSelect { get; set; } = "D";
        public string PopupYn { get; set; } = "Y";
        public string DefaultDeptCD { get; set; } = "";
        public string DefaultEmplNo { get; set; } = "";
        public string receiptState { get; set; } = "W";
        public string QrReceiptYn { get; set; } = "N";
        public string PurposeYn { get; set; } = "Y";
    }
}
