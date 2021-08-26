using JdaTools.Connection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Input;
using Caliburn.Micro;
using Microsoft.Toolkit.Mvvm.Input;
using ICSharpCode.AvalonEdit.Document;
using JdaTools.Studio.AvalonEdit;
using JdaTools.Studio.Helpers;

namespace JdaTools.Studio.ViewModels
{
    public class EditorViewModel : ViewModelBase
    {
        private MocaClient _mocaClient;

        public EditorViewModel(MocaClient mocaClient)
        {
            _mocaClient = mocaClient;
            SetInfoBar("Circle", "Gray", false, "");
        }

        public EditorViewModel(MocaClient mocaClient, string query)
        {
            _mocaClient = mocaClient;
            QueryDocument.Text = query;
            SetInfoBar("Circle", "Gray", false, "");
        }

        public MocaHighlightingDefinition HighlightingDefinition
        {
            get;
        } = IoC.Get<MocaHighlightingDefinition>();


        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private DataTable _resultData;

        public DataTable ResultData
        {
            get => _resultData;
            set => SetProperty(ref _resultData, value);
        }

        private bool _shouldShowResultData { get; set; } = false;
        public bool ShouldShowResultData
        {
            get => _shouldShowResultData;
            set
            {
                _shouldShowResultData = value;
                NotifyOfPropertyChange(() => ShouldShowResultData);
            }
        }
        private string _title = "NEW EDITOR";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private string _localPath;
        public string LocalPath
        {
            get => _localPath;
            set
            {
                _localPath = value;
                NotifyOfPropertyChange(()=>LocalPath);
            }
        }

        private string _remotePath;
        public string RemotePath
        {
            get => _remotePath;
            set
            {
                _remotePath = value;
                NotifyOfPropertyChange(() => RemotePath);
            }
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

                ShouldShowResultData = true;
                SetInfoBar("Circle", "SpringGreen", false, $"SUCCESS : Returned {ResultData.Rows.Count} Rows");
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
        private void SetInfoBar(string iconName, string iconColor, bool isSpinning, string info)
        {
            InfoBarIcon = iconName;
            InfoBarIsRotating = isSpinning;
            InfoBarColor = iconColor;
            InfoBarText = info;
        }
        private TextDocument queryDocument = new();
        public TextDocument QueryDocument { get => queryDocument; set => SetProperty(ref queryDocument, value); }

        
        public async void Upload()
        {
            var dialogueAccepted = await Helpers.DialogueHelper.ShowDialogueYesNo($"UPLOAD {Title}", $"Are you sure you want to upload to {RemotePath} on server?\r\nThis will overwrite file if it exist.");
            if (dialogueAccepted)
            {
                var result = await _mocaClient.ExecuteQuery("write output file where path = @path and data = @data", new
                {
                    path = RemotePath.Substring(0, RemotePath.LastIndexOf("\\", StringComparison.Ordinal)),
                    filename = Title,
                    data = QueryDocument.Text
                });
                if (result.status != 0)
                {
                    await DialogueHelper.ShowDialogueYesNo("ERROR UPLOADING", $"//TODO: {result.message}");
                }
            }
        }
    }
}
