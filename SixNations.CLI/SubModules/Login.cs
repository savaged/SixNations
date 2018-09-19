﻿using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using SixNations.API.Exceptions;
using SixNations.API.Interfaces;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;
using SixNations.Data.Models;

namespace SixNations.CLI.Modules
{
    public class Login : BaseModule, ISubModule
    {
        public async Task RunAsync()
        {
            var email = Entry.Read("Email");
            var password = Entry.Read("Password", true);
            var token = string.Empty;
            try
            {
                token = await SimpleIoc.Default.GetInstance<IAuthTokenService>()
                    .GetTokenAsync(email, password);
            }
            catch (AuthServiceException ex)
            {
                Feedback.Show(ex, Formats.Danger);
            }
            if (!string.IsNullOrEmpty(token))
            {
                User.Current.Initialise(token);
            }
        }
    }
}
