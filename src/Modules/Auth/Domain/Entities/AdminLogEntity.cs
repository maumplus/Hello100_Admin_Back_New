using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Auth.Domain.Entities
{
    public class AdminLogEntity
    {
        public string Aid { get; set; }
        public string UserAgent { get; set; }
        public string IP { get; set; }
    }
}
