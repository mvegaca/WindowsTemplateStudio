using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Templates.UI.Extensions;

namespace Microsoft.Templates.UI.Models
{
    public class AzADApplication
    {
        public string DisplayName { get; set; }

        public string ObjectId { get; set; }

        public IList<string> IdentifierUris { get; set; }

        public string HomePage { get; set; }

        public string Type { get; set; }

        public Guid ApplicationId { get; set; }

        public bool AvailableToOtherTenants { get; set; }

        public IList<string> AppPermissions { get; set; }

        public IList<string> ReplyUrls { get; set; }

        public AzADApplication()
        {
        }

        public AzADApplication(PSObject psobject)
        {
            DisplayName = psobject.GetProperty<string>(nameof(DisplayName));
            ObjectId = psobject.GetProperty<string>(nameof(ObjectId));
            IdentifierUris = psobject.GetProperty<IList<string>>(nameof(IdentifierUris));
            HomePage = psobject.GetProperty<string>(nameof(HomePage));
            Type = psobject.GetProperty<string>(nameof(Type));
            ApplicationId = psobject.GetProperty<Guid>(nameof(ApplicationId));
            AvailableToOtherTenants = psobject.GetProperty<bool>(nameof(AvailableToOtherTenants));
            AppPermissions = psobject.GetProperty<IList<string>>(nameof(AppPermissions));
            ReplyUrls = psobject.GetProperty<IList<string>>(nameof(ReplyUrls));
        }
    }
}
