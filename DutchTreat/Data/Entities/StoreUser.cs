using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data.Entities
{
    public class StoreUser : IdentityUser
    {
        // Add additional properties to the IdentityUser class
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
