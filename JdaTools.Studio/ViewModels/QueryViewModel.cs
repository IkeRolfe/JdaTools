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
            SetInfoBar("Circle", "White", false, "");
        }

        public QueryViewModel(string query)
        {
            _mocaClient = Ioc.Default.GetService<MocaClient>();
            QueryDocument.Text = query;
            SetInfoBar("Circle", "White", false, "");
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

        #region InfoBarStuff
        private string _infoBarIcon;
        public string InfoBarIcon
        {
            get => _infoBarIcon;
            set => SetProperty(ref _infoBarIcon, value);
        }
        private string _infoBarColor;
        public string InfoBarColor
        {
            get => _infoBarColor;
            set => SetProperty(ref _infoBarColor, value);
        }
        private bool _infoBarIsRotating;
        public bool InfoBarIsRotating
        {
            get => _infoBarIsRotating;
            set => SetProperty(ref _infoBarIsRotating, value);
        }

        private string _infoBarText;
        public string InfoBarText
        {
            get => _infoBarText;
            set => SetProperty(ref _infoBarText, value);
        }
        #endregion

        private ICommand executeCommand;
        public ICommand ExecuteCommand => executeCommand ??= new AsyncRelayCommand(Execute);

        internal async Task Execute()
        {
            IsBusy = true;
            SetInfoBar("Autorenew", "DarkSlateBlue", true, "Executing Query");
            var query = QueryDocument.Text;
            if (string.IsNullOrEmpty(query))
            {
                return;
            }
            var result = await _mocaClient.ExecuteQuery(query);
            if (result.status != 0)
            {
                //QueryDocument.Text = $"ERROR {result.status}: {result.message}";
                SetInfoBar("Circle", "Red", false, $"ERROR {result.status}: {result.message}");
            }
            else
            {
                //ResultData = result.MocaResults.GetDataTable();
                try
                {
                    var dt = await ConvertNullValues(result.MocaResults.GetDataTable());
                    ResultData = dt;
                }
                catch (Exception e)
                {
                    ResultData = result.MocaResults.GetDataTable();
                }
            }
            IsBusy = false;
            SetInfoBar("Circle", "SpringGreen", false, $"SUCCESS : Returned {ResultData.Rows.Count} Rows");
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
        private void SetInfoBar(string iconName, string iconColor, bool isSpinning, string info)
        {
            InfoBarIcon = iconName;
            InfoBarIsRotating = isSpinning;
            InfoBarColor = iconColor;
            InfoBarText = info;
        }
        private TextDocument queryDocument = new();
        public TextDocument QueryDocument { get => queryDocument; set => SetProperty(ref queryDocument, value); }

    }
}
