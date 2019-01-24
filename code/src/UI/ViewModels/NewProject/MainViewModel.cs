// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.Templates.Core;
using Microsoft.Templates.Core.Diagnostics;
using Microsoft.Templates.Core.Gen;
using Microsoft.Templates.UI.Controls;
using Microsoft.Templates.UI.Mvvm;
using Microsoft.Templates.UI.Resources;
using Microsoft.Templates.UI.Services;
using Microsoft.Templates.UI.Threading;
using Microsoft.Templates.UI.ViewModels.Common;
using Microsoft.Templates.UI.Views.Common;
using Microsoft.Templates.UI.Views.NewProject;

namespace Microsoft.Templates.UI.ViewModels.NewProject
{
    public class MainViewModel : BaseMainViewModel
    {
        private RelayCommand _refreshTemplatesCacheCommand;
        private RelayCommand _compositionToolCommand;

        private TemplateInfoViewModel _selectedTemplate;

        public static MainViewModel Current { get; private set; }

        public ProjectTypeViewModel ProjectType { get; } = new ProjectTypeViewModel(() => Current.IsSelectionEnabled(MetadataType.ProjectType), () => Current.OnProjectTypeSelected());

        public FrameworkViewModel Framework { get; } = new FrameworkViewModel(() => Current.IsSelectionEnabled(MetadataType.Framework), () => Current.OnFrameworkSelected());

        public AddPagesViewModel AddPages { get; } = new AddPagesViewModel();

        public IdentityConfigViewModel IdentityConfig { get; } = new IdentityConfigViewModel();

        public AddFeaturesViewModel AddFeatures { get; } = new AddFeaturesViewModel();

        public AddTestingViewModel AddTesting { get; } = new AddTestingViewModel();

        public AddServicesViewModel AddServices { get; } = new AddServicesViewModel();

        public UserSelectionViewModel UserSelection { get; } = new UserSelectionViewModel();

        public CompositionToolViewModel CompositionTool { get; } = new CompositionToolViewModel();

        public RelayCommand RefreshTemplatesCacheCommand => _refreshTemplatesCacheCommand ?? (_refreshTemplatesCacheCommand = new RelayCommand(
             () => SafeThreading.JoinableTaskFactory.RunAsync(async () => await OnRefreshTemplatesCacheAsync())));

        public RelayCommand CompositionToolCommand => _compositionToolCommand ?? (_compositionToolCommand = new RelayCommand(() => OnCompositionTool()));

        private static IEnumerable<Step> NewProjectSteps
        {
            get
            {
                yield return Step.MainStep(0, StringRes.NewProjectStepOne, () => new ProjectTypePage(), true, true);
                yield return Step.MainStep(1, StringRes.NewProjectStepTwo, () => new FrameworkPage());
                yield return Step.MainStep(2, StringRes.NewProjectStepThree, () => new AddPagesPage());
                yield return Step.MainStep(3, StringRes.NewProjectStepFour, () => new AddFeaturesPage());
                yield return Step.MainStep(4, StringRes.NewProjectStepFive, () => new AddTestingPage());
                yield return Step.MainStep(5, StringRes.NewProjectStepSix, () => new AddServicesPage());
            }
        }

        public MainViewModel(Window mainView, BaseStyleValuesProvider provider)
            : base(mainView, provider, NewProjectSteps)
        {
            Current = this;
            ValidationService.Initialize(UserSelection.GetNames, UserSelection.GetPageNames);
            Navigation.OnFinish += OnFinish;
        }

        public override void UnsubscribeEventHandlers()
        {
            base.UnsubscribeEventHandlers();
            Navigation.OnFinish -= OnFinish;
        }

        private void OnFinish(object sender, EventArgs e)
        {
            WizardShell.Current.Result = UserSelection.GetUserSelection();
        }

        public override async Task InitializeAsync(string platform, string language)
        {
            WizardStatus.Title = $" ({GenContext.Current.ProjectName})";
            await base.InitializeAsync(platform, language);
        }

        public override bool IsSelectionEnabled(MetadataType metadataType)
        {
            bool result = false;
            if (!UserSelection.HasItemsAddedByUser)
            {
                result = true;
            }
            else
            {
                var vm = new QuestionDialogViewModel(metadataType);
                var questionDialog = new QuestionDialogWindow(vm);
                questionDialog.Owner = WizardShell.Current;
                questionDialog.ShowDialog();

                if (vm.Result == DialogResult.Accept)
                {
                    UserSelection.ResetUserSelection();
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            if (result == true)
            {
                AddPages.ResetUserSelection();
                AddFeatures.ResetTemplatesCount();
            }

            return result;
        }

        public TemplateInfoViewModel GetTemplate(ITemplateInfo templateInfo)
        {
            var groups = templateInfo.GetTemplateType() == TemplateType.Page ? AddPages.Groups : AddFeatures.Groups;
            foreach (var group in groups)
            {
                var template = group.GetTemplate(templateInfo);
                if (template != null)
                {
                    return template;
                }
            }

            return null;
        }

        private void AddTemplate(TemplateInfoViewModel selectedTemplate)
        {
            if (selectedTemplate.MultipleInstance || !UserSelection.IsTemplateAdded(selectedTemplate))
            {
                UserSelection.Add(TemplateOrigin.UserSelection, selectedTemplate);
            }
        }

        protected override Task OnTemplatesAvailableAsync()
        {
            ProjectType.LoadData(Platform);
            ShowNoContentPanel = !ProjectType.Items.Any();
            return Task.CompletedTask;
        }

        public override void ProcessItem(object item)
        {
            if (item is MetadataInfoViewModel metadata)
            {
                if (metadata.MetadataType == MetadataType.ProjectType)
                {
                    ProjectType.Selected = metadata;
                }
                else if (metadata.MetadataType == MetadataType.Framework)
                {
                    Framework.Selected = metadata;
                }
            }
            else if (item is TemplateInfoViewModel template)
            {
                _selectedTemplate = template;
                AddTemplate(template);
            }
        }

        private void OnProjectTypeSelected()
        {
            Framework.LoadData(ProjectType.Selected.Name, Platform);
        }

        private void OnFrameworkSelected()
        {
            AddPages.LoadData(Framework.Selected.Name, Platform);
            AddFeatures.LoadData(Framework.Selected.Name, Platform);
            UserSelection.Initialize(ProjectType.Selected.Name, Framework.Selected.Name, Platform, Language);
            WizardStatus.IsLoading = false;
        }

        protected async Task OnRefreshTemplatesCacheAsync()
        {
            try
            {
                WizardStatus.IsLoading = true;
                UserSelection.ResetUserSelection();
                await GenContext.ToolBox.Repo.RefreshAsync(true);
            }
            catch (Exception ex)
            {
                await NotificationsControl.AddNotificationAsync(Notification.Error(StringRes.NotificationSyncError_Refresh));

                await AppHealth.Current.Error.TrackAsync(ex.ToString());
                await AppHealth.Current.Exception.TrackAsync(ex);
            }
            finally
            {
                WizardStatus.IsLoading = GenContext.ToolBox.Repo.SyncInProgress;
            }
        }

        private void OnCompositionTool()
        {
            var compositionTool = new CompositionToolWindow(UserSelection.GetUserSelection());
            compositionTool.Owner = WizardShell.Current;
            compositionTool.ShowDialog();
        }

        private bool _showNoContentPanel;

        public bool ShowNoContentPanel
        {
            get => _showNoContentPanel;
            set => SetProperty(ref _showNoContentPanel, value);
        }
    }
}
