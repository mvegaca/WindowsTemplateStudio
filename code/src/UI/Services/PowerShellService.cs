// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Templates.UI.Models;
using PS = System.Management.Automation;
using PSR = System.Management.Automation.Runspaces;

namespace Microsoft.Templates.UI.Services
{
    public partial class PowerShellService
    {
        private const string _restrictedExecutionPolicy = "Restricted";

        private readonly PSR.InitialSessionState _iss;

        public PowerShellService()
        {
            _iss = PSR.InitialSessionState.CreateDefault();
        }

        public bool IsExecutionPolicyRestricted()
        {
            var settings = new PSCommandSettings() { FileName = "GetExecutionPolicy" };
            var result = Run(settings);
            return result.Data.Any(r => r.ToString() == _restrictedExecutionPolicy);
        }

        public void SetExecutionPolicyUnrestricted()
        {
            var settings = new PSCommandSettings() { FileName = "SetExecutionPolicyUnrestricted" };
            var result = Run(settings);
        }

        public void ImportModule()
        {
            var settings = new PSCommandSettings() { FileName = "ImportModule" };
            var result = Run(settings);
        }

        public void DisconnectAzAccount(string account)
        {
            var settings = new PSCommandSettings() { FileName = "DisconnectAzAccount" };
            settings.Params.Add(new PSParam("Username", account));
            var result = Run(settings);
        }

        public AzAccount ConnectAzAccount()
        {
            AzAccount user = null;
            var psUserSettings = new PSCommandSettings() { FileName = "ConnectAzAccount" };
            var psUserResult = Run(psUserSettings);
            if (psUserResult.Success)
            {
                var psUserData = psUserResult.Data.FirstOrDefault();
                if (psUserData != null)
                {
                    user = new AzAccount(psUserData);
                    var psTenantesSettings = new PSCommandSettings() { FileName = "GetAzTenant" };
                    var psTenantesResult = Run(psTenantesSettings);
                    if (psTenantesResult.Success)
                    {
                        foreach (var tenant in psTenantesResult.Data)
                        {
                            user.Tenants.Add(new AzTenant(tenant));
                        }

                        user.HasTenants = user.Tenants.Any();
                    }
                }
            }

            return user;
        }

        public AzAccount SetTenant(string tenantId)
        {
            AzAccount user = null;
            var psUserSettings = new PSCommandSettings() { FileName = "SetAzContext" };
            psUserSettings.FileParameters.Add("##tenant##", tenantId);
            var psUserResult = Run(psUserSettings);

            return user;
        }

        public List<AzADApplication> GetAzADApplication()
        {
            List<AzADApplication> apps = new List<AzADApplication>();
            var psAppsSettings = new PSCommandSettings() { FileName = "GetAzADApplication" };
            var psAppsResult = Run(psAppsSettings);
            if (psAppsResult.Success)
            {
                foreach (var app in psAppsResult.Data)
                {
                    apps.Add(new AzADApplication(app));
                }
            }

            return apps;
        }

        private PSResult Run(PSCommandSettings commandSettings)
        {
            PSResult result = new PSResult();
            try
            {
                using (var rs = PSR.RunspaceFactory.CreateRunspace(_iss))
                {
                    rs.Open();
                    var ps = PS.PowerShell.Create();
                    ps.Runspace = rs;
                    commandSettings.ApplySettings(ps);
                    result.Data = ps.Invoke();
                    if (ps.HadErrors)
                    {
                        result.Errors = ps.Streams.Error;
                    }

                    rs.Close();
                    return result;
                }
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
}
