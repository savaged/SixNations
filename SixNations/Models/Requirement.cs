using System;

namespace SixNations.Models
{
    public class Requirement
    {
        public long RequirementID { get; set; }        

        public string Story { get; set; }

        public string EstimationName { get; set; }

        public string StatusName { get; set; }

        public string PriorityName { get; set; }

        public string Release { get; set; }
    }
}
