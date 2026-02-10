using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Auth.Domain.Entities
{
    public class AdminLogEntity
    {
        public string Aid { get; set; } = default!;
        public string UserAgent { get; set; } = default!;
        public string IP { get; set; } = default!;
    }
}
