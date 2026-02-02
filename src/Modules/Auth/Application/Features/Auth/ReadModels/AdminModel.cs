using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.ReadModels
{
    public class AdminModel
    {
        public string Aid { get; set; }
        public string AccId { get; set; }
        public string AccPwd { get; set; }
        public string HospNo { get; set; }
        public string HospKey { get; set; }
        public string Grade { get; set; }
        public string Name { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string Use2fa { get; set; }
        public string AccountLocked { get; set; }
        public int LoginFailCount { get; set; }
        public string LastLoginDt { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
