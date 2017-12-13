using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public abstract class AuthenticationProviderBase
    {
        public readonly string ProviderId;
        public readonly Dictionary<string, string> Parameters = new Dictionary<string, string>();

        public AuthenticationProviderBase(string providerId)
        {
            ProviderId = providerId;
        }

        public abstract Task<AuthenticationResult> AuthenticateAsync();

        public void ConfigureParameter(string parameterName, string parameterValue)
        {
            if (Parameters.ContainsKey(parameterName))
            {
                Parameters[parameterName] = parameterValue;
            }
            else
            {
                Parameters.Add(parameterName, parameterValue);
            }
        }
    }
}
