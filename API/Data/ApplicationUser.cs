using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Organization { get; set; }
        public string Position { get; set; }
        public string SenderName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
