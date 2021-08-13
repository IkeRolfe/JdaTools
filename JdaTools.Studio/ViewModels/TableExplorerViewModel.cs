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
using Caliburn.Micro;

namespace JdaTools.Studio.ViewModels
{
    public class TableExplorerViewModel : ViewModelBase
    {
        private MocaClient _mocaClient;
        private SchemaExplorer _schemaExplorer;

        public TableExplorerViewModel()
        {
            _mocaClient = Ioc.Default.GetService<MocaClient>();
            _schemaExplorer = Ioc.Default.GetService<SchemaExplorer>();
        }



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

        private ICommand refreshCommand;
        public ICommand RefreshCommand => refreshCommand ??= new RelayCommand(Refresh);

        private async void Refresh()
        {
            IsBusy = true;
            await _schemaExplorer.RefreshTables();
            NotifyOfPropertyChange(nameof(Tables));
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

        private ICommand generateSelect;
        public ICommand GenerateSelect => generateSelect ??= new RelayCommand<string>(t => PerformGenerateSelect(t));

        internal void PerformGenerateSelect(string tableName)
        {
            //TODO: Move to messaging service
            var shellView = App.Current.MainWindow;
            var vm = (ShellViewModel)shellView.DataContext;
            vm.NewEditor($"[select top 100 * from {tableName}]", true);
        }

        private object selectedTable;
        public object SelectedTable { get => selectedTable; set => SetProperty(ref selectedTable, value); }

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
    }
}
