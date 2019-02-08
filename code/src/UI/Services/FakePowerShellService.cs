// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Templates.UI.Models;

namespace Microsoft.Templates.UI.Services
{
    public class FakePowerShellService
    {
        public AzAccount ConnectAzAccount()
        {
            var user = new AzAccount()
            {
                Account = "v-maveca@microsoft.com",
                HasTenants = true,
            };
            user.Tenants.Add(new AzTenant() { Id = "0000-0000-0000-0000" });
            user.Tenants.Add(new AzTenant() { Id = "1111-1111-1111-1111" });
            return user;
        }

        public void DisconnectAzAccount(string account)
        {
        }
    }
}
