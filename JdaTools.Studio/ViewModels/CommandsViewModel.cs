using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdaTools.Studio.ViewModels
{
    using JdaTools.Connection;
    using Microsoft.Toolkit.Mvvm.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Runtime.CompilerServices;
    using Microsoft.Toolkit.Mvvm.ComponentModel;
    using System.Windows.Input;
    using Microsoft.Toolkit.Mvvm.Input;
    using JdaTools.Studio.Services;
    using JdaTools.Studio.Models;
    using JdaTools.Studio.Views;
    public class CommandsViewModel : ViewModelBase
    {
        private MocaClient _mocaClient;
        private SchemaExplorer _schemaExplorer;

        public CommandsViewModel()
        {
            _mocaClient = Ioc.Default.GetService<MocaClient>();
            _schemaExplorer = Ioc.Default.GetService<SchemaExplorer>();
        }



        public IEnumerable<CommandDefinition> Commands => GetFilteredCommands();

        private IEnumerable<CommandDefinition> GetFilteredCommands()
        {
            IEnumerable<CommandDefinition> commands;
            if (string.IsNullOrEmpty(SearchString))
            {
                commands = _schemaExplorer.Commands;
            }
            else if (SearchString?.FirstOrDefault() == '^')
            {
                commands = _schemaExplorer.Commands?.Where(t => t.CommandName.StartsWith(SearchString[1..], StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                commands = _schemaExplorer.Commands?.Where(t => t.CommandName.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)); //Apply filter from search
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
            NotifyOfPropertyChange(nameof(Commands));
            IsBusy = false;
        }

        private string _searchString;

        public string SearchString
        {
            get => _searchString;
            set
            {
                SetProperty(ref _searchString, value);
                NotifyOfPropertyChange(nameof(Commands));
            }
        }

        private ICommand generateSelect;
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

        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
    }
}
