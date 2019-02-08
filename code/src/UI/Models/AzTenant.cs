// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Management.Automation;
using Microsoft.Templates.UI.Extensions;

namespace Microsoft.Templates.UI.Models
{
    public class AzTenant
    {
        public string Id { get; set; }

        public string Directory { get; set; }

        public AzTenant()
        {
        }

        public AzTenant(PSObject psobject)
        {
            Id = psobject.GetProperty<string>(nameof(Id));
            Directory = psobject.GetProperty<string>(nameof(Directory));
        }
    }
}
