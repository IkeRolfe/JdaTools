using JdaTools.Connection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using ICSharpCode.AvalonEdit.Document;

namespace JdaTools.Studio.ViewModels
{
    class QueryViewModel : ObservableObject
    {
        private MocaClient _mocaClient;

        public QueryViewModel()
        {
            _mocaClient = Ioc.Default.GetService<MocaClient>();
        }

        public QueryViewModel(string query)
        {
            _mocaClient = Ioc.Default.GetService<MocaClient>();
            QueryDocument.Text = query;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private DataTable resultData;

        public DataTable ResultData
        {
            get => resultData;
            set => SetProperty(ref resultData, value);
        }
        private string _title = "NEW EDITOR";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private ICommand executeCommand;
        public ICommand ExecuteCommand => executeCommand ??= new AsyncRelayCommand(Execute);

        internal async Task Execute()
        {
            IsBusy = true;
            var query = QueryDocument.Text;
            if (string.IsNullOrEmpty(query))
            {
                return;
            }
            var result = await _mocaClient.ExecuteQuery(query);
            if (result.status != 0)
            {
                QueryDocument.Text = $"ERROR {result.status}: {result.message}";
            }
            else
            {
                //ResultData = result.MocaResults.GetDataTable();
                var dt = await ConvertNullValues(result.MocaResults.GetDataTable());
                ResultData = dt;
            }
            IsBusy = false;
        }
        private async Task<DataTable> ConvertNullValues(DataTable dataTable)
        {
            List<string> dcNames = dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            foreach(DataRow row in dataTable.Rows)
            {
                foreach(string columnName in dcNames)
                {
                    row[columnName] = string.IsNullOrEmpty(row[columnName].ToString()) ? "" : row[columnName];
                }
            }

            return dataTable;
        }
        private TextDocument queryDocument = new();
        public TextDocument QueryDocument { get => queryDocument; set => SetProperty(ref queryDocument, value); }

    }
}
