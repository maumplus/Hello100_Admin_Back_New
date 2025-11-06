using System;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.BuildingBlocks.Common.Domain;
using System.Collections.Generic;

namespace Hello100Admin.Modules.Admin.Application.UnitTests.Builders
{
    public static class MemberFamilyTestData
    {
        public static MemberFamily Default => new MemberFamily
        {
            Uid = "F0000001",
            Mid = 2,
            Name = EncryptedData.FromEncrypted("가족1"),
            Birthday = EncryptedData.FromEncrypted("20000101"),
            Sex = EncryptedData.FromEncrypted("M"),
            RegDt = new DateTime(2020, 1, 1, 12, 0, 0)
        };

        public static List<MemberFamily> DefaultList => new List<MemberFamily> { Default };
    }
}
