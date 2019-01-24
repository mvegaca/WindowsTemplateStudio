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
        private StepData _origStep;
        private StepData _currentStep;

        private RelayCommand _cancelCommand;
        private RelayCommand _goBackCommand;
        private RelayCommand _goForwardCommand;
        private RelayCommand _finishCommand;

        public RelayCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(Cancel));

        public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack, CanGoBack));

        public RelayCommand GoForwardCommand => _goForwardCommand ?? (_goForwardCommand = new RelayCommand(GoForward, CanGoForward));

        public RelayCommand FinishCommand => _finishCommand ?? (_finishCommand = new RelayCommand(Finish, CanFinish));

        public ObservableCollection<StepData> Steps { get; }

        public Func<StepData, Task<bool>> IsStepAvailable { get; set; }

        public StepData CurrentStep
        {
            get => _currentStep;
        }

        public event EventHandler OnFinish;

        public event EventHandler<StepData> OnStepUpdated;

        public WizardNavigation(Window wizardShell, IEnumerable<StepData> steps, bool canFinish)
        {
            Current = this;
            _wizardShell = wizardShell;
            _canFinish = canFinish;
            Steps = new ObservableCollection<StepData>(steps);
            SetStepAsync(Steps.First()).FireAndForget();
        }

        private bool CanGoBack() => _canGoBack && !WizardStatus.Current.IsBusy;

        private bool CanGoForward() => _canGoForward && !WizardStatus.Current.IsBusy;

        private bool CanFinish() => _canFinish && !WizardStatus.Current.IsBusy;

        public void Cancel() => _wizardShell.Close();

        private async void GoBack()
        {
            var stepIndex = Steps.IndexOf(CurrentStep);
            await SetStepAsync(Steps.ElementAt(stepIndex - 1));
        }

        private async void GoForward()
        {
            var stepIndex = Steps.IndexOf(CurrentStep);
            await SetStepAsync(Steps.ElementAt(stepIndex + 1));
        }

        private void Finish()
        {
            OnFinish?.Invoke(this, EventArgs.Empty);
            _wizardShell.DialogResult = true;
            _wizardShell.Close();
        }

        public StepData GetCurrentStep() => GetStep(CurrentStep);

        private StepData GetStep(StepData step) => Steps.FirstOrDefault(s => s.Equals(step));

        public async Task SetStepAsync(StepData newStep, bool navigate = true)
        {
            _origStep = _currentStep;
            if (newStep != _currentStep)
            {
                _currentStep = newStep;
            }

            if (IsStepAvailable != null)
            {
                if (!await IsStepAvailable(GetStep(newStep)))
                {
                    DispatcherService.BeginInvoke(() =>
                    {
                        _currentStep = _origStep;
                        OnPropertyChanged(nameof(CurrentStep));
                    });

                    return;
                }
            }

            OnPropertyChanged(nameof(CurrentStep));
            UpdateStep(navigate);
        }

        private void UpdateStep(bool navigate)
        {
            var compleatedSteps = Steps.Where(s => IsPrevious(s, CurrentStep));
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

        private bool IsPrevious(StepData value1, StepData value2)
        {
            var index1 = Steps.IndexOf(value1);
            var index2 = Steps.IndexOf(value2);
            return index1 < index2;
        }

        private void UpdateBackForward()
        {
            var stepIndex = Steps.IndexOf(CurrentStep);
            _canGoBack = stepIndex > 0;
            _canGoForward = stepIndex < Steps.Count - 1;
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
                SetStepAsync(step, false).FireAndForget();
            }
        }

        public void AddNewStep(string stepId, string mainStepId)
        {
            StepData step = null;
            switch (stepId)
            {
                case "Identity":
                    step = StepData.SubStep(stepId, Steps.Count, StringRes.IdentityStepTitle, () => new IdentityConfigPage());
                    break;
                default:
                    return;
            }

            if (step != null)
            {
                var fatherStep = Steps.First(s => s.Id == mainStepId);
                var fatherStepIndex = Steps.IndexOf(fatherStep);
                Steps.Insert(fatherStepIndex + 1, step);
                UpdateBackForward();
            }
        }

        public async Task RemoveStepAsync(string stepId)
        {
            var stepToRemove = Steps.FirstOrDefault(s => s.Id == stepId);
            if (stepToRemove != null)
            {
                var stepToRestore = stepToRemove.Index - 1;
                Steps.Remove(stepToRemove);
                if (stepToRemove.IsSelected)
                {
                    await SetStepAsync(Steps.ElementAt(stepToRestore));
                }
                else
                {
                    UpdateBackForward();
                }
            }
        }

        public void RemoveAllSubSteps()
        {
            Steps.RemoveAll((s) => s.IsSubStep == true);
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
