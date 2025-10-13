using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepremSafe.Core.Entities
{
    public class Earthquake
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EventId { get; set; } //kandilliden gelen id
        public DateTime OccurredAt { get; set; }
        public string Location { get; set; } //yer
        public double Magnitude { get; set; } //büyüklük
        public double Latitude { get; set; } //enlem
        public double Longitude { get; set; } //boylam
        public double Depth { get; set; } //derinlik    

    }
}
