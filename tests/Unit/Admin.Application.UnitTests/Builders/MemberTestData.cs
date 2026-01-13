using System;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.BuildingBlocks.Common.Domain;

namespace Hello100Admin.Modules.Admin.Application.UnitTests.Builders
{
    public static class MemberTestData
    {
        public static MemberEntity Default => new MemberEntity
        {
            Uid = "U0000001",
            Mid = 1,
            Name = EncryptedData.FromEncrypted("홍길동"),
            LoginType = "E",
            LoginTypeName = "이메일",
            RegDtView = "2025-10-29 10:00:00",
            LastLoginDt = DateTime.Now,
            UserRole = 0
        };
    }
}
