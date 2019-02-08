// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Management.Automation;

namespace Microsoft.Templates.UI.Extensions
{
    public static class PSObjectExtensions
    {
        public static T GetProperty<T>(this PSObject psobject, string propertyName)
        {
            var member = psobject.Members[propertyName];
            return member != null ? (T)member.Value : default(T);
        }
    }
}
