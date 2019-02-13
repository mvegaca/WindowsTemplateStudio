// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.Templates.UI.Models;

namespace Microsoft.Templates.UI.Services
{
    public class FakePowerShellService
    {
        public void ImportModule()
        {
        }

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

        public List<AzADApplication> GetAzADApplication()
        {
            var app0 = new AzADApplication()
            {
                ObjectId = "00000000-0000-0000-0000-000000000000",
                ApplicationId = new Guid("00000000-0000-0000-0000-000000000000"),
                DisplayName = "SampleApp0",
            };

            var app1 = new AzADApplication()
            {
                ObjectId = "11111111-1111-1111-1111-111111111111",
                ApplicationId = new Guid("11111111-1111-1111-1111-111111111111"),
                DisplayName = "SampleApp1",
            };

            return new List<AzADApplication>()
            {
                app0,
                app1,
            };
        }

        public void DisconnectAzAccount(string account)
        {
        }

        public void SetTenant(string id)
        {
        }

        public bool IsExecutionPolicyRestricted()
        {
            return false;
        }

        public void SetExecutionPolicyUnrestricted()
        {
        }
    }
}
