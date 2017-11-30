using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using WtsAppAuthentication.Helpers;
using WtsAppAuthentication.Services;
using WtsAppAuthentication.Views;

namespace WtsAppAuthentication.ViewModels
{
    public class AuthenticationViewModel : Observable
    {
        private string _email;
        private string _password;
        private ICommand _emailLoginCommand;
        private ICommand _facebookLoginCommand;
        private ICommand _googleLoginCommand;
        private ICommand _forgotPasswordCommand;

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

        public ICommand EmailLoginCommand => _emailLoginCommand ?? (_emailLoginCommand = new RelayCommand(OnEmailLogin));

        public ICommand FacebookLoginCommand => _facebookLoginCommand ?? (_facebookLoginCommand = new RelayCommand(OnFacebookLogin));

        public ICommand GoogleLoginCommand => _googleLoginCommand ?? (_googleLoginCommand = new RelayCommand(OnGoogleLogin));

        public ICommand ForgotPasswordCommand => _forgotPasswordCommand ?? (_forgotPasswordCommand = new RelayCommand(OnForgotPassword));

        public AuthenticationViewModel()
        {
        }

        private async void OnEmailLogin()
        {
            await LoginAsync(new EmailAuthenticationProvider(Email, Password));
        }

        private async void OnFacebookLogin()
        {
            string clientID = "";
            await LoginAsync(new FacebookAuthenticationProvider(clientID));
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
            AuthenticationService.Initialize(provider);
            var result = await AuthenticationService.AuthenticateAsync();
            if (result.Success)
            {
                NavigationService.Navigate(typeof(ShellPage));
                NavigationService.Navigate(typeof(MainPage));
            }
        }
    }
}
