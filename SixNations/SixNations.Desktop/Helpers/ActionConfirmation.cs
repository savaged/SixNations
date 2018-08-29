using System.Windows;

namespace SixNations.Desktop.Helpers
{
    public static class ActionConfirmation
    {
        public static bool Confirm(ActionConfirmations action)
        {
            var confirmation = MessageBox.Show(
                $"Are you sure you want to {action.ToString()}?",
                action.ToString(),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            return confirmation == MessageBoxResult.Yes;
        }
    }

    public enum ActionConfirmations
    {
        Delete
    }
}
