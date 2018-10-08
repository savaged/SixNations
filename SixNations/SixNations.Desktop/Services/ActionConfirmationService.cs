using System.Windows;
using SixNations.Desktop.Interfaces;

namespace SixNations.Desktop.Services
{
    public class ActionConfirmationService : IActionConfirmationService
    {
        public bool Confirm(ActionConfirmations action)
        {
            var confirmation = MessageBox.Show(
                $"Are you sure you want to {action.ToString()}?",
                action.ToString(),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            return confirmation == MessageBoxResult.Yes;
        }
    }
}
