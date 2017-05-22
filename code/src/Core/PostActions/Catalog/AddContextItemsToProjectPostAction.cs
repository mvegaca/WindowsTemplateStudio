using Microsoft.Templates.Core.Gen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Templates.Core.PostActions.Catalog
{
    public class AddContextItemsToProjectPostAction : PostAction
    {
        public override void Execute()
        {
            GenContext.ToolBox.Shell.ShowStatusBarMessage(Strings.Resources.StatusAddingItems);
            GenContext.ToolBox.Shell.AddItems(GenContext.Current.ProjectItems.ToArray());
            GenContext.Current.ProjectItems.Clear();
        }
    }
}
