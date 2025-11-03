using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepremSafe.Core.DTOs
{
    public class StartChatRequest
    {
        public string UserId { get; set; }
        public bool IsSafe { get; set; }
    }
}
