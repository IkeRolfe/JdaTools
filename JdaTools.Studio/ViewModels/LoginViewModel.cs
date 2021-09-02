using JdaTools.Connection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ControlzEx.Theming;
using Microsoft.Toolkit.Mvvm.Input;
using JdaTools.ConfigurationManager;
using JdaTools.Studio.EventAggregatorMessages;
using JdaTools.Studio.Helpers;
using JdaTools.Studio.Models;
using Action = System.Action;

namespace JdaTools.Studio.ViewModels
{
    public class LoginViewModel : Screen
    {
        private MocaClient _mocaClient;
        private string _userName;
        private string _password;    
        private string _endPointName;
        private string _endPoint;
        private XMLManager _xmlManager;
        private ConnectionInfo _selectedConnection;
        private readonly IEventAggregator _eventAggregator;
        private bool _isDarkModeEnabled;

        public LoginViewModel(MocaClient mocaClient, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _mocaClient = mocaClient;
            _xmlManager = new XMLManager();
            _xmlManager = new XMLManager();
            _xmlManager.ReadConfigurationFile("ConnectionConfigs.xml");
            SelectedConnection = _xmlManager._configs.Connections.FirstOrDefault(c => c.Default);
            _isDarkModeEnabled = AppDataSettings.Default.IsDarkModeEnabled;
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
                _selectedConnection = value;
                NotifyOfPropertyChange(()=>SelectedConnection);
            }
        }
        public string EndpointName
        {
            get => _endPointName;
            set
            {
                _endPointName = value;
                NotifyOfPropertyChange(() => EndpointName);
            }
        }
        public string Endpoint
        {
            get => _endPoint;
            set
            {
                _endPoint = value;
                NotifyOfPropertyChange(() => Endpoint);
            }
        }
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }
        

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        public string AppVersion => "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public string LogoColor => AppDataSettings.Default.IsDarkModeEnabled ? "White" : "Gray";

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
            if (mocaResponse.status != 0)
            {
                UserName = string.Empty;
                Password = string.Empty;
                //TODO show login fail
                return;
            }

            await _eventAggregator.PublishOnUIThreadAsync(EventMessages.LoginEvent);
            LoginCompleteAction?.Invoke();
            await TryCloseAsync(true);
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
