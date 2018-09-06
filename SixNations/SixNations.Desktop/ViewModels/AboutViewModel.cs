using System;
using GalaSoft.MvvmLight;
using System.Reflection;
using SixNations.Desktop.Helpers;

namespace SixNations.Desktop.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            AppNameText = $"{Assembly.GetEntryAssembly().GetName().Name}";
            AppBlurbText = $"An open source WPF application, for requirements management, " +
                $"loosely based on SCRUM.{Environment.NewLine}{AppNameText} is designed " +
                $"as lean tooling for an Agile development team.";
            AppSourceCodeRepoText = "Source code repository is hosted at " +
                "https://github.com/savaged/SixNations";
            AppCopyrightText = $"Copyright (c) 2018 David Savage.{Environment.NewLine}" +
            $"Please see the contents of the file 'COPYING'.{Environment.NewLine}{Environment.NewLine}" +
            $"SixNations is free software: you can redistribute it and / or modify{Environment.NewLine}" +
            $"it under the terms of the GNU General Public License as published{Environment.NewLine}" +
            $"by the Free Software Foundation, either version 3 of the License,{Environment.NewLine}" +
            $"or(at your option) any later version.{Environment.NewLine}{Environment.NewLine}" +
            $"SixNations is distributed in the hope that it will be useful,{Environment.NewLine}" +
            $"but WITHOUT ANY WARRANTY; without even the implied warranty of{Environment.NewLine}" +
            $"MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the{Environment.NewLine}" +
            $"GNU General Public License for more details.{Environment.NewLine}{Environment.NewLine}" +
            $"You should have received a copy of the GNU General Public License{Environment.NewLine}" +
            $"along with SixNations.If not, see <http://www.gnu.org/licenses/>.";
            AppVersionText = $"{AppNameText} v{Assembly.GetEntryAssembly().GetName().Version}";
            OSVersionText = $"Operating System {Environment.OSVersion.VersionString}";
            DotNetVersionText = $".Net v{Environment.Version.ToString()}";
            LogFileLocationText = $"Log file is at {LogFileLocator.GetLogFileLocation()}";
        }

        public string AppNameText { get; }

        public string AppBlurbText { get; }

        public string AppSourceCodeRepoText { get; }

        public string AppCopyrightText { get; }

        public string AppVersionText { get; }

        public string OSVersionText { get; }

        public string DotNetVersionText { get; }

        public string LogFileLocationText { get; }
    }
}
