// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Templates.UI.Services
{
    public class PSParam
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public PSParam(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
