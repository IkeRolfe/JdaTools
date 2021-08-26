using JdaTools.Connection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using JdaTools.Studio.Services;
using JdaTools.Studio.Models;
using JdaTools.Studio.Views;
using Caliburn.Micro;
using JdaTools.Studio.EventAggregatorMessages;
using JdaTools.Studio.Helpers;

namespace JdaTools.Studio.ViewModels
{
    public class TablesViewModel : ToolsetViewBase, IHandle<string>
    {
        public TablesViewModel(MocaClient mocaClient, SchemaExplorer schemaExplorer, IEventAggregator eventAggregator) : base(mocaClient, schemaExplorer, eventAggregator)
        {
        }

        public override string DisplayName { get; set; } = "TABLES";

        public IEnumerable<TableDefinition> Tables => GetFilteredTables();

        private IEnumerable<TableDefinition> GetFilteredTables()
        {
            if (string.IsNullOrEmpty(SearchString))
            {
                return _schemaExplorer.Tables;
            }
            if (SearchString.FirstOrDefault() == '^')
            {
                return _schemaExplorer.Tables?.Where(t => t.TableName.StartsWith(SearchString[1..], StringComparison.InvariantCultureIgnoreCase));
            }
            return _schemaExplorer.Tables?.Where(t => t.TableName.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)); //Apply filter from search

        }

        
        public async void RefreshTables()
        {
            IsBusy = true;
            await _schemaExplorer.RefreshTables();
            NotifyOfPropertyChange(()=>Tables);
            IsBusy = false;
        }

        private string _searchString;

        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                NotifyOfPropertyChange(nameof(Tables));
            }
        }

        
        internal void PerformGenerateSelect(string tableName)
        {
            //TODO: Move to messaging service
            var shellView = App.Current.MainWindow;
            var vm = (ShellViewModel)shellView.DataContext;
            vm.NewEditor($"[select top 100 * from {tableName}]", true);
        }

        private TableDefinition _selectedTable;
        public TableDefinition SelectedTable
        {
            get => _selectedTable;
            set 
            {

                _selectedTable = value;
                NotifyOfPropertyChange(() => SelectedTable);
            }
        }

        public async Task HandleAsync(string message, CancellationToken cancellationToken)
        {
            switch (message)
            {
                case EventMessages.LoginEvent:
                    RefreshTables();
                    break;
            }
        }
    }
}
