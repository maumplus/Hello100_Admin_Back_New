using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Auth.Domain.Entities
{
    public class AppAuthNumberInfoEntity
    {
        public int AuthId { get; set; }
        public string AppCd { get; set; }
        public string Key { get; set; }
        public string AuthNumber { get; set; }
        public string ConfirmYn { get; set; }
        public int ModUnixDt { get; set; }
        public string ModDt { get; set; }
        public int RegUnixDt { get; set; }
        public string RegDt { get; set; }
    }
}
