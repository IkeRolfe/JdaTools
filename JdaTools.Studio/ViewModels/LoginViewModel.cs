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
using JdaTools.ConfigurationManager;

namespace JdaTools.Studio.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private MocaClient _mocaClient;
        private string _userName;
        private string _password;    
        private string _endPointName;
        private string _endPoint;
        private XMLManager _xmlManager;
        private ConnectionInfo _selectedConnection;

        public LoginViewModel(MocaClient mocaClient)
        {
            _mocaClient = mocaClient;
            _xmlManager = new XMLManager();
        }

        public LoginViewModel()
        {
            _mocaClient = Ioc.Default.GetService<MocaClient>();
            _xmlManager = new XMLManager();
            _xmlManager.ReadConfigurationFile("ConnectionConfigs.xml");
            SelectedConnection = _xmlManager._configs.Connections.FirstOrDefault(c => c.Default);

        }

        public List<ConnectionInfo> Connections => _xmlManager._configs.Connections;
        public ConnectionInfo SelectedConnection
        {
            get
            {
                return _selectedConnection;
            }
            set
            {
                EndpointName = value.Name;
                Endpoint = value.Url;
                SetProperty(ref _selectedConnection, value);
            }
        }
        public string EndpointName
        {
            get => _endPointName;
            set
            {
                SetProperty(ref _endPointName, value);
            }
        }
        public string Endpoint
        {
            get => _endPoint;
            set
            {
                SetProperty(ref _endPoint, value);                
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
            _mocaClient.Endpoint = Endpoint;
            await SaveConnection();
            var mocaResponse = await _mocaClient.ConnectAsync(new MocaCredentials(UserName, Password));
            LoginCompleteAction?.Invoke();
                    
        }

        public async Task SaveConnection()
        {
            SelectedConnection.Name = EndpointName;
            SelectedConnection.Url = Endpoint;
            
            await _xmlManager.SetDefault(SelectedConnection);
            await _xmlManager.WriteConfigurationFile("ConnectionConfigs.xml");
        }

        private ICommand loginCommand;
        public ICommand LoginCommand => loginCommand ??= new AsyncRelayCommand(Login);
        
    }
}
