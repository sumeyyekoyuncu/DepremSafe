using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepremSafe.Core.Entities
{
    public class UserLocation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public double Latitude { get; set; } //enlem
        public double Longitude { get; set; } //boylam
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; //en son konumun alındığı zaman
        public string Source { get; set; } //konumun kaynağı (GPS, WiFi, Hücresel,BLE Relay,son bilinen)
        public User User { get; set; } //navigation property
    }
}
