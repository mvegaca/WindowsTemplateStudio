// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Templates.UI.Models;
using Microsoft.Templates.UI.Mvvm;
using Microsoft.Templates.UI.Services;
using Microsoft.Templates.UI.Views.Common;
using Microsoft.Templates.UI.Views.NewProject;

namespace Microsoft.Templates.UI.ViewModels.Common
{
    public class IdentityConfigViewModel : Observable
    {
        public const string _azureSignUpUrl = "https://azure.microsoft.com/free/";
        public const string _azurePortalUrl = "https://portal.azure.com/";

        private bool _isBusy;
        private bool _isLoggedIn;
        private RelayCommand _loginCommand;
        private RelayCommand _logOutCommand;
        private RelayCommand _signUpCommand;
        private RelayCommand _openAzurePortalCommand;
        private PowerShellService _powerShell = new PowerShellService();
        private FakePowerShellService _fakePowerShell = new FakePowerShellService();
        private AzAccount _account;
        private AzTenant _tenant;

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
            set { SetProperty(ref _tenant, value); }
        }

        public RelayCommand LoginCommand => _loginCommand ?? (_loginCommand = new RelayCommand(OnLogin, () => !IsBusy));

        public RelayCommand LogOutCommand => _logOutCommand ?? (_logOutCommand = new RelayCommand(OnLogOut, () => !IsBusy));

        public RelayCommand SignUpCommand => _signUpCommand ?? (_signUpCommand = new RelayCommand(OnSignUp, () => !IsBusy));

        public RelayCommand OpenAzurePortalCommand => _openAzurePortalCommand ?? (_openAzurePortalCommand = new RelayCommand(OnOpenAzurePortal, () => !IsBusy));

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

                //Account = _powerShell.ConnectAzAccount();
                Account = _fakePowerShell.ConnectAzAccount();
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
                //_powerShell.DisconnectAzAccount(Account.Account);
                _fakePowerShell.DisconnectAzAccount(Account.Account);
                if (Account.HasTenants)
                {
                    Tenant = Account.Tenants.First();
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
            var isRestricted = _powerShell.IsExecutionPolicyRestricted();
            if (isRestricted)
            {
                var changeExecutionPolicy = AnswerExecutionPolicyChange();
                if (!changeExecutionPolicy)
                {
                    return true;
                }

                _powerShell.SetExecutionPolicyUnrestricted();
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
    }
}
