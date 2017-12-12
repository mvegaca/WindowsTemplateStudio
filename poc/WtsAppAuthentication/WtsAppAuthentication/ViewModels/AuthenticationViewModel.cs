using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.ApplicationSettings;
using WtsAppAuthentication.Helpers;
using WtsAppAuthentication.Services;
using WtsAppAuthentication.Views;

namespace WtsAppAuthentication.ViewModels
{
    public class AuthenticationViewModel : Observable
    {
        private bool _isLoading;
        private bool _rememberCredentials;
        private string _loginErrorMessage;
        private string _registerErrorMessage;
        private string _email;
        private string _newEmail;
        private string _password;
        private string _newPassword;
        private string _newPasswordConfirmation;
        private RelayCommand _emailLoginCommand;
        private RelayCommand _microsoftLoginCommand;
        private RelayCommand _facebookLoginCommand;
        private RelayCommand _twitterLoginCommand;
        private RelayCommand _googleLoginCommand;
        private RelayCommand _forgotPasswordCommand;
        private RelayCommand _registerCommand;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                Set(ref _isLoading, value);
                EmailLoginCommand.OnCanExecuteChanged();
                MicrosoftLoginCommand.OnCanExecuteChanged();
                FacebookLoginCommand.OnCanExecuteChanged();
                TwitterLoginCommand.OnCanExecuteChanged();
                GoogleLoginCommand.OnCanExecuteChanged();
                ForgotPasswordCommand.OnCanExecuteChanged();
                RegisterCommand.OnCanExecuteChanged();
            }
        }

        public bool RememberCredentials
        {
            get => _rememberCredentials;
            set => Set(ref _rememberCredentials, value);
        }

        public string LoginErrorMessage
        {
            get => _loginErrorMessage;
            set => Set(ref _loginErrorMessage, value);
        }

        public string RegisterErrorMessage
        {
            get => _registerErrorMessage;
            set => Set(ref _registerErrorMessage, value);
        }

        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        public string NewEmail
        {
            get => _newEmail;
            set => Set(ref _newEmail, value);
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => Set(ref _newPassword, value);
        }

        public string NewPasswordConfirmation
        {
            get => _newPasswordConfirmation;
            set => Set(ref _newPasswordConfirmation, value);
        }

        public RelayCommand EmailLoginCommand => _emailLoginCommand ?? (_emailLoginCommand = new RelayCommand(OnEmailLogin, () => !IsLoading));

        public RelayCommand MicrosoftLoginCommand => _microsoftLoginCommand ?? (_microsoftLoginCommand = new RelayCommand(OnMicrosoftLogin, () => !IsLoading));

        public RelayCommand FacebookLoginCommand => _facebookLoginCommand ?? (_facebookLoginCommand = new RelayCommand(OnFacebookLogin, () => !IsLoading));

        public RelayCommand TwitterLoginCommand => _twitterLoginCommand ?? (_twitterLoginCommand = new RelayCommand(OnTwitterLogin, () => !IsLoading));

        public RelayCommand GoogleLoginCommand => _googleLoginCommand ?? (_googleLoginCommand = new RelayCommand(OnGoogleLogin, () => !IsLoading));

        public RelayCommand ForgotPasswordCommand => _forgotPasswordCommand ?? (_forgotPasswordCommand = new RelayCommand(OnForgotPassword, () => !IsLoading));

        public RelayCommand RegisterCommand => _registerCommand ?? (_registerCommand = new RelayCommand(OnRegister, () => !IsLoading));

        public AuthenticationViewModel()
        {
        }

        public void LoadData()
        {
            Email = AuthenticationService.Data.Email;
            Password = AuthenticationService.Data.Password;
            RememberCredentials = AuthenticationService.Data.RememberCredentials;
        }

        private async void OnEmailLogin()
        {
            if(IsValidEmailLogin())
            {
                await LoginAsync(new EmailAuthenticationProvider(Email, Password));
                AuthenticationService.Data.RememberCredentials = RememberCredentials;
                AuthenticationService.Data.Email = RememberCredentials ? Email : string.Empty;
                AuthenticationService.Data.Password = RememberCredentials ? Password : string.Empty;
                await AuthenticationService.SaveDataAsync();
            }
        }

        private async void OnMicrosoftLogin()
        {
            await LoginAsync(new MicrosoftAuthenticationProvider());
        }

        private async void OnFacebookLogin()
        {
            // TODO WTS: Add your Facebook Client ID
            var clientID = "";
            await LoginAsync(new FacebookAuthenticationProvider(clientID));
        }

        private async void OnTwitterLogin()
        {
            // TODO WTS: Add your Twitter Consumer Key, Consumer Secret and CallBack URL
            var consumerKey = "";
            var consumerSecret = "";
            var callbackURL = "https://github.com/Microsoft/WindowsTemplateStudio/";
            await LoginAsync(new TwitterAuthenticationProvider(consumerKey, consumerSecret, callbackURL));
        }

        private async void OnGoogleLogin()
        {
            await LoginAsync(new GoogleAuthenticationProvider());
        }

        private void OnForgotPassword()
        {
        }

        private async void OnRegister()
        {
            IsLoading = true;
            try
            {
                if (IsValidEmailRegister())
                {
                    // WTS: TODO register with backend API
                    AuthenticationService.Data.Email = NewEmail;
                    AuthenticationService.Data.Password = NewPassword;
                    AuthenticationService.Data.RememberCredentials = true;
                    await SuccessLoginAsync();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoginAsync(IAuthenticationProvider provider)
        {
            try
            {
                IsLoading = true;
                LoginErrorMessage = string.Empty;
                var result = await provider.AuthenticateAsync(OnPrivacyPolicyInvoked);
                if (result.Success)
                {
                    await SuccessLoginAsync();
                }
                else
                {
                    switch (result.Reason)
                    {
                        case Models.ReasonType.UserCancel:
                            break;
                        case Models.ReasonType.ErrorHttp:
                            LoginErrorMessage = "ErrorLoginHttp".GetLocalized();
                            break;
                        case Models.ReasonType.Unexpected:
                            //TODO WTS: Look at result.ErrorMessage to find more information about the error
                            LoginErrorMessage = "ErrorLoginUnexpected".GetLocalized();
                            break;
                    }
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SuccessLoginAsync()
        {
            AuthenticationService.Data.IsLoggedIn = true;
            await AuthenticationService.SaveDataAsync();

            // ShellPage will replace the NavigationService Frame
            NavigationService.Navigate(typeof(ShellPage));
            // Then we navigate to MainPage on Shell Frame
            NavigationService.Navigate(typeof(MainPage));
        }

        private bool IsValidEmailLogin()
        {
            if (string.IsNullOrEmpty(Email))
            {
                LoginErrorMessage = "ErrorLoginEmailEmpty".GetLocalized();
                return false;
            }
            else if (string.IsNullOrEmpty(Password))
            {
                LoginErrorMessage = "ErrorLoginPasswordEmpty".GetLocalized();
                return false;
            }
            else
            {
                LoginErrorMessage = string.Empty;
                return true;
            }
        }

        private bool IsValidEmailRegister()
        {
            if (string.IsNullOrEmpty(NewEmail))
            {
                RegisterErrorMessage = "ErrorRegisterEmailEmpty".GetLocalized();
                return false;
            }
            else if (string.IsNullOrEmpty(NewPassword))
            {
                RegisterErrorMessage = "ErrorRegisterPasswordEmpty".GetLocalized();
                return false;
            }
            else if (string.IsNullOrEmpty(NewPasswordConfirmation))
            {
                RegisterErrorMessage = "ErrorRegisterPasswordConfirmationEmpty".GetLocalized();
                return false;
            }
            else if (NewPassword != NewPasswordConfirmation)
            {
                RegisterErrorMessage = "ErrorRegisterPasswordConfirmationDoNotMatch".GetLocalized();
                return false;
            }
            else if (NewPassword.Length < 8 || NewPassword.Length > 20)
            {
                RegisterErrorMessage = "ErrorRegisterPasswordLength".GetLocalized();
                return false;
            }
            else
            {
                RegisterErrorMessage = string.Empty;
                return true;
            }
        }

        private async void OnPrivacyPolicyInvoked()
        {
            IsLoading = false;
            await Launcher.LaunchUriAsync(new Uri("https://aka.ms/wts"));
        }
    }
}
