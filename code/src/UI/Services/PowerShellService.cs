// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.PowerShell.Commands;
using Microsoft.Templates.UI.Models;
using PS = System.Management.Automation;
using PSR = System.Management.Automation.Runspaces;

namespace Microsoft.Templates.UI.Services
{
    public partial class PowerShellService
    {
        private readonly PSR.InitialSessionState _iss;

        public PowerShellService()
        {
            _iss = PSR.InitialSessionState.CreateDefault();
        }

        public bool IsExecutionPolicyRestricted()
        {
            var result = Run("GetExecutionPolicy.ps1");
            return result.Data.Any(r => r.ToString() == "Restricted");
        }

        public void SetExecutionPolicyUnrestricted()
        {
            Run("SetExecutionPolicyUnrestricted.ps1");
        }

        public void DisconnectAzAccount(string account)
        {
            Run("DisconnectAzAccount.ps1", account);
        }

        public AzAccount ConnectAzAccount()
        {
            AzAccount user = null;
            var psUserResult = Run("ConnectAzAccount.ps1");
            if (psUserResult.Success)
            {
                var psUserData = psUserResult.Data.FirstOrDefault();
                if (psUserData != null)
                {
                    user = new AzAccount(psUserData);
                    var psTenantesResult = Run("GetAzTenant.ps1");
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

        private Task<PSResult> RunAsync(string scriptName)
        {
            var tcs = new TaskCompletionSource<PSResult>();
            try
            {
                using (var rs = PSR.RunspaceFactory.CreateRunspace(_iss))
                {
                    rs.Open();
                    var ps = PS.PowerShell.Create();
                    ps.Runspace = rs;
                    var scriptFile = FileService.Read($@"Assets\PS\{scriptName}");
                    ps.AddScript(scriptFile);
                    var settings = new PS.PSInvocationSettings();
                    object state = null;
                    PSResult result = null;
                    PS.PSDataCollection<PS.PSObject> data = null;
                    ps.BeginInvoke<object, PS.PSObject>(null, data, settings, (ar) =>
                    {
                        rs.Close();
                        result.Data = new Collection<PS.PSObject>(data.ToList());
                        tcs.SetResult(result);
                    }, state);
                }
            }
            catch (Exception)
            {
            }

            return tcs.Task;
        }

        private PSResult Run(string scriptName, string parameter = null)
        {
            PSResult result = new PSResult();
            try
            {
                using (var rs = PSR.RunspaceFactory.CreateRunspace(_iss))
                {
                    rs.Open();
                    var ps = PS.PowerShell.Create();
                    ps.Runspace = rs;
                    var scriptFile = FileService.Read($@"Assets\PS\{scriptName}");
                    ps.AddScript(scriptFile);
                    if (parameter != null)
                    {
                        ps.AddParameter(parameter);
                    }

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
