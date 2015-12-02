using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.Services.Content
{
    public class SitecoreUserItem
    {
        public string UserName { get; set; }
        public string FullUserName { get; set; }
        public string FullName { get; set; }
        public string Domain { get; set; }
        public string Roles { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
    }
}
