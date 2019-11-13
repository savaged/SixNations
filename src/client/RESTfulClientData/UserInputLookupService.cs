using System;
using System.IO;
using log4net;
using Newtonsoft.Json;
using System.Reflection;

namespace savaged.mvvm.Data
{
    public class UserInputLookupService : IUserInputLookupService
    {
        private static readonly ILog Log = LogManager
            .GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _userInputLookupLocation;

        public UserInputLookupService(
            string userInputLookupLocation)
        {
            _userInputLookupLocation = userInputLookupLocation;
        }

        public IUserInputLookup Get(
            string lookupName, string selectedItem)
        {
            ValidateArgument(nameof(lookupName), lookupName);

            var filename = GetUserInputLookupFileName(lookupName);
            var lookup = new UserInputLookup(selectedItem);
            var json = string.Empty;
            UserInputLookup tmp;
            try
            {
                json = File.ReadAllText(filename);
            }
            catch (FileNotFoundException)
            {
                return lookup;
            }
            try
            {
                tmp = JsonConvert
                    .DeserializeObject<UserInputLookup>(json);
            }
            catch (JsonSerializationException ex)
            {
                Log.Error(
                    $"Error deserializing {filename}:" +
                    $"\n\t{ex.Message}");
                return lookup;
            }
            foreach (var value in tmp)
            {
                lookup.Add(value);
            }
            return lookup;
        }

        public bool Update(string lookupName, IUserInputLookup lookup)
        {
            ValidateArgument(nameof(lookupName), lookupName);
            ValidateArgument(nameof(lookup), lookup);

            var filename = GetUserInputLookupFileName(lookupName);
            var json = string.Empty;
            try
            {
                json = JsonConvert.SerializeObject(lookup);
            }
            catch (JsonSerializationException ex)
            {
                Log.Error(
                    $"Error serializing {lookupName}:" +
                    $"\n\t{ex.Message}");
                return false;
            }
            try
            {
                File.WriteAllText(filename, json);
            }
            catch (Exception ex)
            {
                Log.Error(
                    $"Error writing {json} to {filename}:" +
                    $"\n\t{ex.Message}");
                return false;
            }
            return true;
        }

        private void ValidateArgument(string argumentName, object arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        private string GetUserInputLookupFileName(string lookupName)
        {
            return $"{_userInputLookupLocation}{lookupName}.json";
        }

    }
}
