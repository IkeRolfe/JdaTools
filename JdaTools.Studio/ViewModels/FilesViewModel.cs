using JdaTools.Connection;
using JdaTools.Studio.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JdaTools.Studio.ViewModels
{
    class FilesViewModel : ObservableObject
    {
        private MocaClient _mocaClient;
        private SchemaExplorer _schemaExplorer;

        public FilesViewModel()
        {
            _mocaClient = Ioc.Default.GetService<MocaClient>();
            _schemaExplorer = Ioc.Default.GetService<SchemaExplorer>();
        }


        private bool isBusy;
        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
        public IEnumerable<string> Files => GetFilteredCommands();

        private IEnumerable<string> GetFilteredCommands()
        {
            IEnumerable<string> commands;
            if (string.IsNullOrEmpty(SearchString))
            {
                commands = _schemaExplorer.Files;
            }
            else if (SearchString?.FirstOrDefault() == '^')
            {
                commands = _schemaExplorer.Files?.Where(t => t.StartsWith(SearchString, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                commands = _schemaExplorer.Files?.Where(t => t.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)); //Apply filter from search
            }
            //TODO add pagination listview with 10,000 items is slow
            return commands?.Take(100);
        }

        private ICommand refreshCommand;
        public ICommand RefreshCommand => refreshCommand ??= new RelayCommand(Refresh);

        private async void Refresh()
        {
            IsBusy = true;
            await _schemaExplorer.RefreshCommands();
            OnPropertyChanged(nameof(Files));
            IsBusy = false;
        }

        private string _searchString;

        public string SearchString
        {
            get => _searchString;
            set
            {
                SetProperty(ref _searchString, value);
                OnPropertyChanged(nameof(Files));
            }
        }

        /*private ICommand generateSelect;
        public ICommand GenerateSelect => generateSelect ??= new RelayCommand<CommandDefinition>(c => ShowSyntaxCommand(c));

        internal void ShowSyntaxCommand(CommandDefinition command)
        {
            //TODO: Move to messaging service
            var shellView = Ioc.Default.GetService<ShellView>();
            var vm = (ShellViewModel)shellView.DataContext;
            vm.NewEditor(command.Syntax, false);
        }

        private object selectedCommand;
        public object SelectedCommand { get => selectedCommand; set => SetProperty(ref selectedCommand, value); }

        private bool isBusy;

        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }*/
    }
}
