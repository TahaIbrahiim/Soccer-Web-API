using System.Text.Json.Serialization;

namespace SoccerAPI.Models
{
    public class Coach
    {
        public int CoachId { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string description { get; set; }
        public string nationality { get; set; }
        public int age { get; set; }
        public decimal Salary { get; set; }

        [JsonIgnore]
        public virtual ICollection<Club>? Clubs { get; set; } = new HashSet<Club>();
    }
}
