using SixNations.API.Constants;

namespace SixNations.Desktop.Interfaces
{
    public interface ISwimlaneDropTarget
    {
        void OnDrop(int droppedRequirementId, RequirementStatus target);
    }
}