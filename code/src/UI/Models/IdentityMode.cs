// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Templates.UI.ViewModels.Common;

namespace Microsoft.Templates.UI.Models
{
    public class IdentityMode : Selectable
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public IdentityMode(bool isSelected)
            : base(isSelected)
        {
        }
    }
}
