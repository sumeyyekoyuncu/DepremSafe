using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepremSafe.Core.DTOs
{
    public class UpdateCityRequest
    {
        public Guid UserId { get; set; }
        public string City { get; set; }
    }


}
