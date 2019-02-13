using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Templates.UI.Models;

namespace Microsoft.Templates.UI.ViewModels.Common
{
    public class IdentityModesViewModel : SelectableGroup<IdentityMode>
    {
        public IdentityModesViewModel(Func<bool> isSelectionEnabled, Action onSelected)
            : base(isSelectionEnabled, onSelected)
        {
        }
    }
}
