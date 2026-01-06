using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepremSafe.Core.DTOs
{
    public class SendTestNotificationRequest
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Type { get; set; } = "general";
    }
}
