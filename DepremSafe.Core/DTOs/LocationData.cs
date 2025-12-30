using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepremSafe.Core.DTOs
{
    public class LocationData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public float? Accuracy { get; set; }
        public long Timestamp { get; set; }
    }
}
