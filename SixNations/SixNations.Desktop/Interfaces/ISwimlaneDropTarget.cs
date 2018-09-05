using SixNations.Desktop.Constants;

namespace SixNations.Desktop.Interfaces
{
    public interface ISwimlaneDropTarget
    {
        void OnDrop(int droppedRequirementId, RequirementStatus target);
    }
}