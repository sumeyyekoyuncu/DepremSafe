using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepremSafe.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Username { get; set; }

        public string? Email { get; set; }           
        public string? GoogleId { get; set; }        
        public string? LoginProvider { get; set; }    // Google / Local

        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? FcmToken { get; set; }

        public bool? IsSafe { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public virtual ICollection<UserLocation> Locations { get; set; }
    }

}
