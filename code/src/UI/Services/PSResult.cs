// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace Microsoft.Templates.UI.Services
{
    public class PSResult
    {
        //public Collection<PSObject> Data { get; set; }

        //public PSDataCollection<ErrorRecord> Errors { get; set; }

        public string Result { get; set; }

        public string Error { get; set; }

        public int ExitCode { get; set; }

        //public bool Success
        //{
        //    get
        //    {
        //        if (Errors != null && Errors.Any())
        //        {
        //            return false;
        //        }
        //        else if (Data == null)
        //        {
        //            return false;
        //        }

        //        return true;
        //    }
        //}
    }
}
