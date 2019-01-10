
namespace SixNations.Server.Models
{
    public class Requirement
    {
        public int RequirementID { get; set; }

        public string Story { get; set; }

        public string Release { get; set; }

        public int Estimation { get; set; }

        public int Priority { get; set; }

        public int Status { get; set; }
    }
}
