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
        private string _email;
        private string _password;
        private RelayCommand _emailLoginCommand;
        private RelayCommand _microsoftLoginCommand;
        private RelayCommand _facebookLoginCommand;
        private RelayCommand _twitterLoginCommand;
        private RelayCommand _googleLoginCommand;
        private RelayCommand _forgotPasswordCommand;

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
            }
        }

        public bool RememberCredentials
        {
            get => _rememberCredentials;
            set => Set(ref _rememberCredentials, value);
        }

        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public RelayCommand EmailLoginCommand => _emailLoginCommand ?? (_emailLoginCommand = new RelayCommand(OnEmailLogin, () => !IsLoading));

        public RelayCommand MicrosoftLoginCommand => _microsoftLoginCommand ?? (_microsoftLoginCommand = new RelayCommand(OnMicrosoftLogin, () => !IsLoading));

        public RelayCommand FacebookLoginCommand => _facebookLoginCommand ?? (_facebookLoginCommand = new RelayCommand(OnFacebookLogin, () => !IsLoading));

        public RelayCommand TwitterLoginCommand => _twitterLoginCommand ?? (_twitterLoginCommand = new RelayCommand(OnTwitterLogin, () => !IsLoading));        

        public RelayCommand GoogleLoginCommand => _googleLoginCommand ?? (_googleLoginCommand = new RelayCommand(OnGoogleLogin, () => !IsLoading));

        public RelayCommand ForgotPasswordCommand => _forgotPasswordCommand ?? (_forgotPasswordCommand = new RelayCommand(OnForgotPassword, () => !IsLoading));

        public AuthenticationViewModel()
        {
        }

        private async void OnEmailLogin()
        {
            await LoginAsync(new EmailAuthenticationProvider(Email, Password));
        }

        private async void OnMicrosoftLogin()
        {
            await LoginAsync(new MicrosoftAuthenticationProvider());
        }

        private async void OnFacebookLogin()
        {
            // TODO WTS: Add your Facebook Client ID
            var clientID = "338735353201434";
            await LoginAsync(new FacebookAuthenticationProvider(clientID));
        }

        private async void OnTwitterLogin()
        {
            // TODO WTS: Add your Twitter Consumer Key, Consumer Secret and CallBack URL
            var consumerKey = "JmAJn1YEGKaBiKqyT1t7pxv13";
            var consumerSecret = "r0bsqp4JGPo2IIhjnyc3V8aw9nLD83OC3FMPxn4OaxtwGTFcRq";
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

        private async Task LoginAsync(IAuthenticationProvider provider)
        {
            try
            {
                IsLoading = true;
                var result = await provider.AuthenticateAsync(OnPrivacyPolicyInvoked);
                if (result.Success)
                {
                    AuthenticationService.Data.IsLoggedIn = true;
                    await AuthenticationService.SaveDataAsync();
                    NavigationService.Navigate(typeof(ShellPage));
                    NavigationService.Navigate(typeof(MainPage));
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

        private async void OnPrivacyPolicyInvoked()
        {
            IsLoading = false;
            await Launcher.LaunchUriAsync(new Uri("https://aka.ms/wts"));
        }
    }
}
