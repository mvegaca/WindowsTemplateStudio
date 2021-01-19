using System;

namespace WinUI3App.Helpers
{
    public class SuspendAndResumeArgs : EventArgs
    {
        public SuspensionState SuspensionState { get; set; }

        public Type Target { get; private set; }

        public SuspendAndResumeArgs(SuspensionState suspensionState, Type target)
        {
            SuspensionState = suspensionState;
            Target = target;
        }
    }
}
