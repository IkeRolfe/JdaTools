using JdaTools.Connection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows;
using JdaTools.Studio.Services;
using Caliburn.Micro;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using JdaTools.Studio.EventAggregatorMessages;
using JdaTools.Studio.Helpers;
using JdaTools.Studio.Models;
using JdaTools.Studio.Views;
using MahApps.Metro.SimpleChildWindow;
using Application = System.Windows.Application;
using Screen = Caliburn.Micro.Screen;

namespace JdaTools.Studio.ViewModels
{
    public class ShellViewModel : Screen, IHandle<string>
    {
        private readonly MocaClient _mocaClient;
        private LoginViewModel _loginViewModel;
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;


        public ShellViewModel(MocaClient mocaClient, SchemaExplorer schemaExplorer, IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnPublishedThread(this);
            _mocaClient = mocaClient;
            _windowManager = windowManager;
            //TODO move to event aggregator


        }

        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Tools.Add(IoC.Get<TablesViewModel>());
            Tools.Add(IoC.Get<FilesViewModel>());
            Tools.Add(IoC.Get<CommandsViewModel>());
            Editors.Add(new EditorViewModel(_mocaClient));
            Login = IoC.Get<LoginViewModel>();
        }

        protected override async void OnViewLoaded(object view)
        {
            
            
        }

        public LoginViewModel Login
        {
            get => _loginViewModel;
            set
            {
                _loginViewModel = value;
                NotifyOfPropertyChange(()=>Login);
            }
        }

        public ObservableCollection<IScreen> Tools { get; set; } = new();

        private ObservableCollection<EditorViewModel> _editors = new ObservableCollection<EditorViewModel>();

        public ObservableCollection<EditorViewModel> Editors
        {
            get => _editors;
            set
            {
                _editors = value;
                NotifyOfPropertyChange(() => _editors);
            }
        }
        //Needed for hotkey
        private ICommand _newEditorCommand;
        public ICommand NewEditorCommand => _newEditorCommand ??= new RelayCommand(NewEditor);

        public async void Upload()
        {
            await _eventAggregator.PublishOnUIThreadAsync(EventMessages.UploadClickedEvent);
        }
        internal void NewEditor()
        {
            var vm = new EditorViewModel(_mocaClient);
            Editors.Add(vm);
            SelectedEditor = vm;
        }

        
        internal void NewEditor(string query, bool execute = false, string title = null, string localPath = null, string remotePath = null)
        {
            var vm = new EditorViewModel(_mocaClient, query);
            if (title != null)
            {
                vm.Title = title;
                vm.LocalPath = localPath;
                vm.RemotePath = remotePath;
            }
            Editors.Add(vm);
            SelectedEditor = vm;
            if (execute)
            {
                _ = vm.Execute();
            }
        }

        private EditorViewModel _selectedEditor;
        public EditorViewModel SelectedEditor
        {
            get => _selectedEditor;
            set
            {
                _selectedEditor = value;
                NotifyOfPropertyChange(()=>SelectedEditor);
            }
        }

        //TODO: to bool with coverter
        private Visibility loginVisibility = Visibility.Visible;
        public Visibility LoginVisibility
        {
            get => loginVisibility; 
            set
            {
                if (value == Visibility.Visible)
                {
                    IsEnabled = false;
                }
                else
                {
                    IsEnabled = true;
                }
                loginVisibility = value;
                NotifyOfPropertyChange(() => loginVisibility);
            }
        }

        private ICommand _executeCurrentTabCommand;
        public ICommand ExecuteCurrentTabCommand => _executeCurrentTabCommand ??= new RelayCommand(ExecuteCurrentTab);

        private async void ExecuteCurrentTab()
        {
            if (SelectedEditor != null)
            {
                await SelectedEditor.Execute();
            }
        }

        private bool isEnabled;
        

        public bool IsEnabled { 
            get => isEnabled; 
            set
            {
                isEnabled = value;
                NotifyOfPropertyChange(() => isEnabled);
            }
        }

        private ICommand executeCommand;
        public ICommand ExecuteCommand => executeCommand ??= new RelayCommand(ExecuteCurrentTab);

        private ICommand saveCommand;
        public ICommand SaveCommand => saveCommand ??= new RelayCommand(Save);
        private ICommand openCommand;
        
        public ICommand OpenCommand => openCommand ??= new RelayCommand(Open);

        public void Save()
        {
            var path = SelectedEditor?.LocalPath;
            if (string.IsNullOrEmpty(path))
            {
                SaveAs();
                return;
            }
            var text = SelectedEditor?.QueryDocument.Text;
            File.WriteAllText(path,text);
        }

        public void SaveAs()
        {
            var text = SelectedEditor?.QueryDocument.Text;
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = SelectedEditor.Title;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, text);
                var fullPath = saveFileDialog.FileName;
                SelectedEditor.LocalPath = fullPath;
                SelectedEditor.Title = fullPath.Substring(fullPath.LastIndexOf("\\", StringComparison.Ordinal)+1);
            }
                
        }

        public void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var fullPath = openFileDialog.FileName;
                var fileName = fullPath.Substring(fullPath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                NewEditor();
                SelectedEditor.Title = fileName;
                SelectedEditor.LocalPath = fullPath;
                SelectedEditor.QueryDocument.Text = File.OpenText(fullPath).ReadToEnd();
            }
        }

        public bool CanSaveAs() => true;
        public async Task HandleAsync(string message, CancellationToken cancellationToken)
        {
            switch (message)
            {
                case EventMessages.LoginEvent:
                    LoginVisibility = Visibility.Collapsed;
                    break;
                case EventMessages.UploadClickedEvent:
                    SelectedEditor?.Upload();
                    break;
            } 
        }

        
    }
}
