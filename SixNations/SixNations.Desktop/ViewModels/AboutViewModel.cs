using System;
using GalaSoft.MvvmLight;
using System.Reflection;
using SixNations.Desktop.Helpers;
using SixNations.Data.Models;

namespace SixNations.Desktop.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            SelectedItem = new About();
        }

        public About SelectedItem { get; }
    }
}
