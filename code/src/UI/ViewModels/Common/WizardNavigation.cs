// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Templates.Core;
using Microsoft.Templates.UI.Controls;
using Microsoft.Templates.UI.Mvvm;
using Microsoft.Templates.UI.Resources;
using Microsoft.Templates.UI.Services;
using Microsoft.Templates.UI.Views.Common;

namespace Microsoft.Templates.UI.ViewModels.Common
{
    public class WizardNavigation : Observable
    {
        public static WizardNavigation Current { get; private set; }

        private Window _wizardShell;

        private bool _canGoBack = false;
        private bool _canGoForward = true;
        private bool _canFinish;
        private int _origStep;
        private int _step;

        private RelayCommand _cancelCommand;
        private RelayCommand _goBackCommand;
        private RelayCommand _goForwardCommand;
        private RelayCommand _finishCommand;

        public RelayCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(Cancel));

        public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack, CanGoBack));

        public RelayCommand GoForwardCommand => _goForwardCommand ?? (_goForwardCommand = new RelayCommand(GoForward, CanGoForward));

        public RelayCommand FinishCommand => _finishCommand ?? (_finishCommand = new RelayCommand(Finish, CanFinish));

        public ObservableCollection<Step> Steps { get; }

        public Func<Step, Task<bool>> IsStepAvailable { get; set; }

        public int Step
        {
            get => _step;
            private set => SetStepAsync(value).FireAndForget();
        }

        public event EventHandler OnFinish;

        public event EventHandler<Step> OnStepUpdated;

        public WizardNavigation(Window wizardShell, IEnumerable<Step> steps, bool canFinish)
        {
            Current = this;
            _wizardShell = wizardShell;
            _canFinish = canFinish;
            Steps = new ObservableCollection<Step>(steps);
        }

        private bool CanGoBack() => _canGoBack && !WizardStatus.Current.IsBusy;

        private bool CanGoForward() => _canGoForward && !WizardStatus.Current.IsBusy;

        private bool CanFinish() => _canFinish && !WizardStatus.Current.IsBusy;

        public void Cancel() => _wizardShell.Close();

        private void GoBack() => Step--;

        private void GoForward() => Step++;

        private void Finish()
        {
            OnFinish?.Invoke(this, EventArgs.Empty);
            _wizardShell.DialogResult = true;
            _wizardShell.Close();
        }

        public Step GetCurrentStep() => GetStep(Step);

        private Step GetStep(int step) => Steps.FirstOrDefault(s => s.Equals(step));

        public async Task SetStepAsync(int newStep, bool navigate = true)
        {
            _origStep = _step;
            if (newStep != _step)
            {
                _step = newStep;
            }

            if (IsStepAvailable != null)
            {
                if (!await IsStepAvailable(GetStep(newStep)))
                {
                    DispatcherService.BeginInvoke(() =>
                    {
                        _step = _origStep;
                        OnPropertyChanged(nameof(Step));
                    });

                    return;
                }
            }

            OnPropertyChanged(nameof(Step));
            UpdateStep(navigate);
        }

        private void UpdateStep(bool navigate)
        {
            var compleatedSteps = Steps.Where(s => s.IsPrevious(Step));
            foreach (var step in compleatedSteps)
            {
                step.Completed = true;
            }

            foreach (var step in Steps)
            {
                step.IsSelected = false;
            }

            var selectedStep = GetCurrentStep();
            if (selectedStep != null)
            {
                selectedStep.IsSelected = true;
                if (navigate)
                {
                    NavigationService.NavigateSecondaryFrame(selectedStep.GetPage());
                }
            }

            UpdateBackForward();
            OnStepUpdated?.Invoke(this, selectedStep);
        }

        private void UpdateBackForward()
        {
            _canGoBack = Step > 0;
            _canGoForward = Step < Steps.Count - 1;
        }

        public void SetCanFinish(bool canFinish)
        {
            _canFinish = canFinish;
        }

        public void RefreshStep(object navigatedPage)
        {
            var step = Steps.FirstOrDefault(s => s.Equals(navigatedPage.GetType()));
            if (step != null)
            {
                SetStepAsync(step.Index, false).FireAndForget();
            }
        }

        public void AddNewStep(string stepId, bool fromRightClick = false)
        {
            Step step = null;
            switch (stepId)
            {
                case "Identity":
                    step = Microsoft.Templates.UI.Controls.Step.MainStep(Steps.Count, StringRes.IdentityStepTitle, () => new IdentityConfigPage());
                    break;
                default:
                    return;
            }

            if (step != null)
            {
                step.StepId = stepId;
                step.StepType = StepType.AddedByTemplate;
                if (fromRightClick)
                {
                    Steps[1].Index = 2;
                    step.Index = 1;
                    Steps.Insert(1, step);
                }
                else
                {
                    Steps.Add(step);
                }

                UpdateBackForward();
            }
        }

        public async Task RemoveStepAsync(string stepId)
        {
            var stepToRemove = Steps.FirstOrDefault(s => s.StepId == stepId);
            if (stepToRemove != null)
            {
                var stepToRestore = stepToRemove.Index - 1;
                Steps.Remove(stepToRemove);
                if (stepToRemove.IsSelected)
                {
                    await SetStepAsync(stepToRemove.Index - 1);
                }
                else
                {
                    UpdateBackForward();
                }
            }
        }

        public void RemoveAddedByTemplatesSteps()
        {
            Steps.RemoveAll((s) => s.StepType == StepType.AddedByTemplate);
            for (int index = 0; index < Steps.Count; index++)
            {
                if (Steps[index].Index != index)
                {
                    Steps[index].Index = index;
                }
            }
        }
    }
}
