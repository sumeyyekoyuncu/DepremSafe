using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepremSafe.Core.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string FcmToken { get; set; }
        public bool IsSafe { get; set; }
    }
}
