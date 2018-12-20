// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Templates.UI
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null || items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static int RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> condition)
        {
            var itemsToRemove = collection.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                collection.Remove(itemToRemove);
            }

            return itemsToRemove.Count;
        }
    }
}
