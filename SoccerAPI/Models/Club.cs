using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoccerAPI.Models
{
    public class Club
    {
        public int ClubId { get; set; }
        public string name { get; set; }
        public string Owner { get; set; }
        public string description { get; set; }
        public string league { get; set; }
        public string stadium { get; set; }
        public string sponsor { get; set; }

        [ForeignKey("Coach")]
        public int CoachId { get; set; }

        [JsonIgnore]
        public virtual Coach? Coach { get; set; }

        [JsonIgnore]
        public virtual ICollection<Player>? Players { get; set; } = new HashSet<Player>();
    }
}
