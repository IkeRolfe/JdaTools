using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

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
    public class CommandsViewModel : Screen, IHandle<string>
    {
        private MocaClient _mocaClient;
        private SchemaExplorer _schemaExplorer;
        private IEventAggregator _eventAggregator;

        public CommandsViewModel(MocaClient mocaClient, SchemaExplorer schemaExplorer, IEventAggregator eventAggregator)
        {
            _mocaClient = mocaClient;
            _schemaExplorer = schemaExplorer;
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnPublishedThread(this);
        }

        public override string DisplayName { get; set; } = "COMMANDS";

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
            return commands?.Take(200);
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
                _searchString = value;
                NotifyOfPropertyChange(()=>SearchString);
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
        public object SelectedCommand
        {
            get => selectedCommand;
            set {
                selectedCommand = value;
                NotifyOfPropertyChange(() => SelectedCommand);
            }
        }

        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        public async Task HandleAsync(string message, CancellationToken cancellationToken)
        {
            switch (message)
            {
                case EventMessages.LoginEvent:
                    Refresh();
                    break;
            }
        }
    }
}
