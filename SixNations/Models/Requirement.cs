using GalaSoft.MvvmLight;

namespace SixNations.Models
{
    public class Requirement : ObservableObject
    {
        private int _requirementID;
        private string _story;
        private int _estimation;
        private int _priority;
        private int _status;
        private string _release;

        public int RequirementID
        {
            get => _requirementID;
            set => Set(ref _requirementID, value);
        }        

        public string Story
        {
            get => _story;
            set => Set(ref _story, value);
        }

        public string Release
        {
            get => _release;
            set => Set(ref _release, value);
        }

        public int Estimation
        {
            get => _estimation;
            set => Set(ref _estimation, value);
        }

        public int Priority
        {
            get => _priority;
            set => Set(ref _priority, value);
        }

        public int Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        public string EstimationName { get; set; }

        public string StatusName { get; set; }

        public string PriorityName { get; set; }
    }
}
