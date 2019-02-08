// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Templates.UI.Extensions;

namespace Microsoft.Templates.UI.Models
{
    public class AzAccount
    {
        public string Account { get; set; }

        public string TenantId { get; set; }

        public List<AzTenant> Tenants { get; } = new List<AzTenant>();

        public bool HasTenants { get; set;  }

        public AzAccount()
        {
        }

        public AzAccount(PSObject psobject)
        {
            Account = psobject.GetProperty<string>(nameof(Account));
            TenantId = psobject.GetProperty<string>(nameof(TenantId));
        }
    }
}
