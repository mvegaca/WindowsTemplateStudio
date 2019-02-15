// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.Templates.UI.Models;
using Microsoft.Templates.UI.Mvvm;
using Microsoft.Templates.UI.Resources;
using Microsoft.Templates.UI.Services;
using Microsoft.Templates.UI.Views.Common;
using Microsoft.Templates.UI.Views.NewProject;

namespace Microsoft.Templates.UI.ViewModels.Common
{
    public class IdentityConfigViewModel : Observable
    {
        public static IdentityConfigViewModel Current;

        public const string _azureSignUpUrl = "https://azure.microsoft.com/free/";
        public const string _azurePortalUrl = "https://portal.azure.com/";

        private bool _isBusy;
        private bool _isLoggedIn;
        private bool _isAADIdentityMode = true;
        private bool _supportedAccountTypeThisOrg = true;
        private RelayCommand _loginCommand;
        private RelayCommand _logOutCommand;
        private RelayCommand _signUpCommand;
        private RelayCommand _openAzurePortalCommand;
        private PowerShellService _powerShell = new PowerShellService();
        private FakePowerShellService _fakePowerShell = new FakePowerShellService();
        private AzAccount _account;
        private AzTenant _tenant;
        private bool _useFake = false;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                SetProperty(ref _isBusy, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set { SetProperty(ref _isLoggedIn, value); }
        }

        public bool IsAADIdentityMode
        {
            get { return _isAADIdentityMode; }
            set { SetProperty(ref _isAADIdentityMode, value); }
        }

        public bool SupportedAccountTypeThisOrg
        {
            get { return _supportedAccountTypeThisOrg; }
            set { SetProperty(ref _supportedAccountTypeThisOrg, value); }
        }

        public AzAccount Account
        {
            get { return _account; }
            set
            {
                IsLoggedIn = value != null;
                SetProperty(ref _account, value);
            }
        }

        public AzTenant Tenant
        {
            get { return _tenant; }
            set
            {
                SetProperty(ref _tenant, value);
                SetTenant();
            }
        }

        public ObservableCollection<AzADApplication> Applications { get; } = new ObservableCollection<AzADApplication>();

        public RelayCommand LoginCommand => _loginCommand ?? (_loginCommand = new RelayCommand(OnLogin, () => !IsBusy));

        public RelayCommand LogOutCommand => _logOutCommand ?? (_logOutCommand = new RelayCommand(OnLogOut, () => !IsBusy));

        public RelayCommand SignUpCommand => _signUpCommand ?? (_signUpCommand = new RelayCommand(OnSignUp, () => !IsBusy));

        public RelayCommand OpenAzurePortalCommand => _openAzurePortalCommand ?? (_openAzurePortalCommand = new RelayCommand(OnOpenAzurePortal, () => !IsBusy));

        public IdentityModesViewModel IdentityModes { get; } = new IdentityModesViewModel(IsSelectionEnabled, OnSelected);

        public IdentityConfigViewModel()
        {
            Current = this;
            IdentityModes.Items.Add(new IdentityMode(true)
            {
                Title = StringRes.IdentityModeAADTitle,
                Description = StringRes.IdentityModeAADDescription,
            });
            IdentityModes.Items.Add(new IdentityMode(false)
            {
                Title = StringRes.IdentityModeB2BTitle,
                Description = StringRes.IdentityModeB2BDescription,
            });
            IdentityModes.Selected = IdentityModes.Items.First();
        }

        private void OnLogin()
        {
            try
            {
                IsBusy = true;
                if (IsExecutionPolicyRestricted())
                {
                    return;
                }

                // TODO Check
                // ---- POWERSHELL VERSION
                // ---- AZ INSTALLED
                // ---- Import-Module Az

                if (_useFake)
                {
                    _fakePowerShell.ImportModule();
                    Account = _fakePowerShell.ConnectAzAccount();
                }
                else
                {
                    _powerShell.ImportModule();
                    Account = _powerShell.ConnectAzAccount();
                }

                if (Account.HasTenants)
                {
                    Tenant = Account.Tenants.First();
                }
            }
            catch (Exception)
            {
                Account = null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnLogOut()
        {
            try
            {
                IsBusy = true;
                if (_useFake)
                {
                    _fakePowerShell.DisconnectAzAccount(Account.Account);
                }
                else
                {
                    _powerShell.DisconnectAzAccount(Account.Account);
                }

            }
            catch (Exception)
            {
            }
            finally
            {
                Account = null;
                IsBusy = false;
            }
        }

        private void SetTenant()
        {
            if (_useFake)
            {
                _fakePowerShell.SetTenant(Tenant.Id);
                Applications.Clear();
                _fakePowerShell.GetAzADApplication().ForEach(app => Applications.Add(app));
            }
            else
            {
                _powerShell.SetTenant(Tenant.Id);
                Applications.Clear();
                _powerShell.GetAzADApplication().ForEach(app => Applications.Add(app));
            }
        }

        private void OnSignUp()
        {
            Process.Start(_azureSignUpUrl);
        }

        private void OnOpenAzurePortal()
        {
            Process.Start(_azurePortalUrl);
        }

        private bool IsExecutionPolicyRestricted()
        {
            bool isRestricted;
            if (_useFake)
            {
                isRestricted = _fakePowerShell.IsExecutionPolicyRestricted();
            }
            else
            {
                isRestricted = _powerShell.IsExecutionPolicyRestricted();
            }

            if (isRestricted)
            {
                var changeExecutionPolicy = AnswerExecutionPolicyChange();
                if (!changeExecutionPolicy)
                {
                    return true;
                }

                if (_useFake)
                {
                    _fakePowerShell.SetExecutionPolicyUnrestricted();
                }
                else
                {
                    _powerShell.SetExecutionPolicyUnrestricted();
                }
            }

            return false;
        }

        private bool AnswerExecutionPolicyChange()
        {
            var vm = new QuestionDialogViewModel("ExecutionPolicy", "You need to set a unrestricted execution policy to run scripts to connect with Azure. Do you want to set unrestricted execution policy?");
            var questionDialog = new QuestionDialogWindow(vm);
            questionDialog.Owner = WizardShell.Current;
            questionDialog.ShowDialog();

            return vm.Result == DialogResult.Accept;
        }

        public void SelectIdentityMode(IdentityMode identityMode)
        {
            IdentityModes.Selected = identityMode;
            IsAADIdentityMode = IdentityModes.Selected == IdentityModes.Items.First();
        }

        private static bool IsSelectionEnabled()
        {
            return true;
        }

        private static void OnSelected()
        {
        }
    }
}
