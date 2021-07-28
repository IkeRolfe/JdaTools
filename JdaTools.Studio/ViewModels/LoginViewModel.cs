using JdaTools.Connection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

namespace JdaTools.Studio.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private MocaClient _mocaClient;
        private string _userName;
        private string _password;    
        private string _endPoint;

        public LoginViewModel(MocaClient mocaClient)
        {
            _mocaClient = mocaClient;
        }

        public LoginViewModel()
        {
            _mocaClient = Ioc.Default.GetService<MocaClient>();
            Endpoint = "http://wlgprdmoca.edc.ecentria.com:5040/service";
            UserName = "ASUPRPL";
            Password = "1234";
        }
        public string Endpoint
        {
            get => _endPoint;
            set
            {
                SetProperty(ref _endPoint, value);
                _mocaClient.Endpoint = value;
            }
        }
        public string UserName 
        { 
            get => _userName;
            set => SetProperty(ref _userName, value);
        }
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public Action LoginCompleteAction
        {
            get;
            set;
        }

        public async Task Login()
        {
            var mocaResponse = await _mocaClient.ConnectAsync(new MocaCredentials(UserName, Password));
            LoginCompleteAction?.Invoke();
        }

        private ICommand loginCommand;
        public ICommand LoginCommand => loginCommand ??= new AsyncRelayCommand(Login);
        
    }
}
