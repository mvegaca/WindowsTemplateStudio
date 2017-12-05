using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Web.Http.Headers;

namespace WtsAppAuthentication.Services
{
    public static class AuthenticationHelper
    {
        public static string GetRequestParameters(Dictionary<string, string> parameters)
        {
            var requestParameters = string.Empty;
            foreach (var param in parameters)
            {
                if (string.IsNullOrEmpty(param.Key))
                {
                    throw new ArgumentNullException("parameterKey");
                }
                if (string.IsNullOrEmpty(requestParameters))
                {
                    requestParameters = $"{param.Key}={param.Value}";
                }
                else
                {
                    requestParameters += $"&{param.Key}={param.Value}";
                }
            }
            return requestParameters;
        }

        public static HttpCredentialsHeaderValue GetCredentialsHeader(string scheme, Dictionary<string, string> parameters)
        {
            string token = string.Empty;
            if (parameters == null || !parameters.Any())
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            foreach (var param in parameters)
            {
                if (string.IsNullOrEmpty(param.Key))
                {
                    throw new ArgumentNullException("parameterKey");
                }
                token += $"{param.Key}=\"{param.Value}\", ";
            }
            return new HttpCredentialsHeaderValue(scheme, token);
        }
    }
}
