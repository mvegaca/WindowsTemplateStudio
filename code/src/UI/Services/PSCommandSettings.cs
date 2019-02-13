using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using PS = System.Management.Automation;

namespace Microsoft.Templates.UI.Services
{
    public enum PsCommand
    {
        Az,
        Aad,
        Common,
    }

    public class PSCommandSettings
    {
        private const string _powerShellPath = "c:/windows/sysnative/windowspowershell/v1.0/powershell";

        public PsCommand Command { get; set; }

        public string FileName { get; set; }

        public Dictionary<string, string> FileParameters { get; } = new Dictionary<string, string>();

        public List<object> Params { get; } = new List<object>();

        public PSCommandSettings()
        {
        }

        public void ApplySettings(PS.PowerShell ps)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                var script = FileService.Read($@"Assets\PS\{Command}\{FileName}.ps1");
                foreach (var param in FileParameters)
                {
                    script = script.Replace(param.Key, param.Value);
                }

                ps.AddScript(script);
            }

            foreach (var param in Params)
            {
                if (param is string strintParam)
                {
                    ps.AddParameter(strintParam);
                }
                else if (param is PSParam psParam)
                {
                    ps.AddParameter(psParam.Name, psParam.Value);
                }
            }
        }

        public ProcessStartInfo GetProcessInfo()
        {
            // WorkingDirectory = "C:/Users/mvega/Desktop/",
            // Arguments = $".\\All.ps1",
            return new ProcessStartInfo()
            {
                FileName = _powerShellPath,
                WorkingDirectory = FileService.ExecutingDirectory,
                Arguments = $@"Assets\PS\{Command}\{FileName}.ps1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

        }
    }
}
