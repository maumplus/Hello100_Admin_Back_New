namespace Hello100Admin.BuildingBlocks.Common.Definition.Enums
{
    public class GlobalConstant
    {
        public class ContentTypes
        {
            public const string Xlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }

        public class AdminRoles
        {
            public const string SuperAdmin = "S";
            public const string GeneralAdmin = "A";
            public const string HospitalAdmin = "C";
        }

        public class ChartTypes
        {
            public const string EghisChart = "E";
            public const string NixChart = "N";
        }
    }
}
