using JdaTools.Connection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace JdaTools.Studio.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private MocaClient _mocaClient;
        private SchemaExplorer _schemaExplorer;
        private LoginViewModel _loginViewModel = new LoginViewModel();
        private TableExplorerViewModel _tableExplorerViewModel = new TableExplorerViewModel();
        private CommandsViewModel _commandsViewModel = new();
        private FilesViewModel _filesViewModel = new();


        public ShellViewModel()
        {
            _mocaClient = Ioc.Default.GetService<MocaClient>();
            _schemaExplorer = Ioc.Default.GetService<SchemaExplorer>();
            Editors.Add(new EditorViewModel());
            LoginViewModel.LoginCompleteAction = new System.Action(() => OnLoginComplete());
        }
        
        private void OnLoginComplete()
        {
            LoginVisibility = Visibility.Collapsed;
            //Refresh jda schema TODO: move to messaging center
            TableExplorer.RefreshCommand.Execute(null);
            CommandsViewModel.RefreshCommand.Execute(null);
            FilesViewModel.RefreshCommand.Execute(null);
            
        }

        private ObservableCollection<EditorViewModel> _editors = new ObservableCollection<EditorViewModel>();

        public ObservableCollection<EditorViewModel> Editors
        {
            get => _editors;
            set => SetProperty(ref _editors, value);
        }
        public LoginViewModel LoginViewModel
        {
            get => _loginViewModel;
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
            var vm = new EditorViewModel();
            Editors.Add(vm);
            SelectedEditor = vm;
        }

        internal void NewEditor(string query, bool execute = false, string title = null)
        {
            var vm = new EditorViewModel(query);
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

        

    }
}
