using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoccerAPI.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string position { get; set; }
        public string nationality { get; set; }
        public decimal salary { get; set; }
        public int speed { get; set; }
        public decimal tall { get; set; }
        public int strength { get; set; }
        public int agility { get; set; }
        public int passes { get; set; }
        public string gender { get; set; }

        [ForeignKey("Club")]
        public int ClubId { get; set; }

        [JsonIgnore]
        public virtual Club? Club { get; set; }

    }
}
