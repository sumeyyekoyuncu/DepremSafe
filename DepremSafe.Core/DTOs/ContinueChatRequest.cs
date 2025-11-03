using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepremSafe.Core.DTOs
{
    public class ContinueChatRequest
    {
        public string UserId { get; set; }
        public bool IsPositive { get; set; }
    }
}
