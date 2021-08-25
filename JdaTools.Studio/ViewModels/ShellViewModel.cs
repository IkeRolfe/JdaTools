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
using JdaTools.Studio.Models;

namespace JdaTools.Studio.ViewModels
{
    public class ShellViewModel : ViewModelBase, IHandle<string>
    {
        private MocaClient _mocaClient;
        private SchemaExplorer _schemaExplorer;
        private LoginViewModel _loginViewModel;
        private TableExplorerViewModel _tableExplorerViewModel;
        private CommandsViewModel _commandsViewModel;
        private FilesViewModel _filesViewModel;


        public ShellViewModel(MocaClient mocaClient, SchemaExplorer schemaExplorer, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _mocaClient = mocaClient;
            _schemaExplorer = schemaExplorer;
            Editors.Add(new EditorViewModel(_mocaClient));
            //TODO move to event aggregator
            _tableExplorerViewModel = new TableExplorerViewModel(_mocaClient, _schemaExplorer);
            Tools.Add(new FilesViewModel(_mocaClient, _schemaExplorer, _eventAggregator));
            Tools.Add(new CommandsViewModel(_mocaClient, _schemaExplorer, _eventAggregator));
            Login = new LoginViewModel(_mocaClient, _eventAggregator);
        }
        
        private void OnLoginComplete()
        {
            LoginVisibility = Visibility.Collapsed;
            //Refresh jda schema TODO: move to messaging center
            TableExplorer.RefreshCommand.Execute(null);
            CommandsViewModel.RefreshCommand.Execute(null);
            FilesViewModel.RefreshCommand.Execute(null);
            
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

        public ObservableCollection<object> Tools { get; set; } = new ObservableCollection<object>();

        private ObservableCollection<EditorViewModel> _editors = new ObservableCollection<EditorViewModel>();

        public ObservableCollection<EditorViewModel> Editors
        {
            get => _editors;
            set => SetProperty(ref _editors, value);
        }
        
        public TableExplorerViewModel TableExplorer
        {
            get => _tableExplorerViewModel;
        }
        public CommandsViewModel CommandsViewModel
        {
            get => _commandsViewModel;
        }
        public FilesViewModel FilesViewModel
        {
            get => _filesViewModel;
        }

        private ICommand newEditorCommand;
        public ICommand NewEditorCommand => newEditorCommand ??= new RelayCommand(NewEditor);

        internal void NewEditor()
        {
            var vm = new EditorViewModel(_mocaClient);
            Editors.Add(vm);
            SelectedEditor = vm;
        }

        internal void NewEditor(string query, bool execute = false, string title = null)
        {
            var vm = new EditorViewModel(_mocaClient, query);
            if (title != null)
            {
                vm.Title = title;
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
                SetProperty(ref loginVisibility, value);
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
        

        public bool IsEnabled { get => isEnabled; set => SetProperty(ref isEnabled, value); }

        private ICommand executeCommand;
        public ICommand ExecuteCommand => executeCommand ??= new RelayCommand(ExecuteCurrentTab);

        private ICommand saveCommand;
        public ICommand SaveCommand => saveCommand ??= new RelayCommand(Save);
        private ICommand openCommand;
        private readonly IEventAggregator _eventAggregator;
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
            }
        }
    }
}
