using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace WtsAppAuthentication.Extensions
{
    public static class StringExtensions
    {
        public static Dictionary<string, string> ReadParameters(this string responseString, char splitChar, char equalChar)
        {
            var result = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(responseString) && responseString.Contains(splitChar))
            {
                var keyValPairs = responseString.Split('&');
                foreach (var keyValuePair in keyValPairs)
                {
                    if (!string.IsNullOrEmpty(keyValuePair) && keyValuePair.Contains(equalChar))
                    {
                        var splits = keyValuePair.Split('=');
                        if (splits.Count() == 2)
                        {
                            result.Add(splits[0], splits[1]);
                        }
                    }
                }
            }
            return result;
        }
    }
}
