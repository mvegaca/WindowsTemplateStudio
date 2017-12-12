using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WtsAppAuthentication.Models
{
    public enum ReasonType { UserCancel, ErrorHttp, Unexpected }

    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public Dictionary<string, string> ResponseData { get; set; }
        public string ErrorMessage { get; set; }
        public ReasonType Reason { get; set; }
    }
}
