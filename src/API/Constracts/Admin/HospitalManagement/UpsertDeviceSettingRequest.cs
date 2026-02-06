namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class UpsertDeviceSettingRequest
    {
        /// <summary>
        /// 병원명
        /// </summary>
        public string HospNm { get; set; }
        public string EmplNo { get; set; }
        public string DeviceNm { get; set; }
        public int DeviceType { get; set; }
        public string InfoTxt { get; set; }
        public string UseYn { get; set; }
        public DeviceSettingInfo SetJson { get; set; }
    }

    public class DeviceSettingInfo
    {
        public string PayYn { get; set; } = "N";
        public string AddrYn { get; set; } = "N";
        public string DeptYn { get; set; } = "N";
        public string DeptBreakYn { get; set; } = "N";
        public string DetailYn { get; set; } = "N";
        public string ReceptYn { get; set; } = "N";
        public string SimplePayYn { get; set; } = "N";
        public string WaitTimeYn { get; set; } = "N";
        public string? ViewMinTime { get; set; } = "";
        public string NewReceiveYn { get; set; } = "N";
        public string PrtBarcodeYn { get; set; } = "N";
        public int PtntInputType { get; set; } = 1;
        public string ReceiveMainSelect { get; set; } = "D";
        public string PopupYn { get; set; } = "Y";
        public string DefaultDeptCD { get; set; } = "";
        public string DefaultEmplNo { get; set; } = "";
        public string receiptState { get; set; } = "W";
        public string QrReceiptYn { get; set; } = "N";
        public string PurposeYn { get; set; } = "Y";
    }
}
