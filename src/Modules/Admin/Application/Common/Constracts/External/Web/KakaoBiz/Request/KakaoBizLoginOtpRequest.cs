using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.KakaoBiz.Request
{
    public class KakaoBizLoginOtpRequest
    {
        public string PhoneNo { get; set; }
        public string Otp { get; set; }
        public string EncKey { get; set; } = default!;
    }
}
