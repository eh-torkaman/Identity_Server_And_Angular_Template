using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace STS.Models
{


    public partial class DbUser
    {
        public DbUser()
        {
            dbClaims = new List<DbClaim>();
        }
        public string Id { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public int ProfileImageNumber { get; set; }

        public bool IsLockedOut {
            get { 
                return LockoutEnabled&&(LockoutEnd> DateTimeOffset.Now); 
            } 
        }
        public ICollection<DbClaim> dbClaims { get; set; }
    }

    public partial class DbClaim
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public DbUser dbUser { get; set; }
    }

    public partial class VUserClaims
    {
        public string UserName { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public bool IsLockedOut
        {
            get
            {
                return LockoutEnabled && (LockoutEnd > DateTimeOffset.Now);
            }
        }
    }
}
