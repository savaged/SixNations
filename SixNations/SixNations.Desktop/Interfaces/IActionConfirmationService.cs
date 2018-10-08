namespace SixNations.Desktop.Interfaces
{
    public interface IActionConfirmationService
    {
        bool Confirm(ActionConfirmations action);
    }

    public enum ActionConfirmations
    {
        Cancel,
        Delete
    }
}