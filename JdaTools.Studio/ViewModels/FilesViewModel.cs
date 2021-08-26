using JdaTools.Connection;
using JdaTools.Studio.Services;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Caliburn.Micro;
using JdaTools.Studio.EventAggregatorMessages;
using JdaTools.Studio.Helpers;
using JdaTools.Studio.Models;
using MahApps.Metro.SimpleChildWindow;

namespace JdaTools.Studio.ViewModels
{
    public class FilesViewModel : Screen, IHandle<string>
    {
        private readonly MocaClient _mocaClient;
        private readonly SchemaExplorer _schemaExplorer;
        
        public FilesViewModel(MocaClient mocaClient, SchemaExplorer schemaExplorer, IEventAggregator eventAggregator)
        {
            _mocaClient = mocaClient;
            _schemaExplorer = schemaExplorer;
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnPublishedThread(this);
        }

        public override string DisplayName { get; set; } = "FILES";

        public override string ToString()
        {
            return DisplayName;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(()=>IsBusy);
            }
        }

        private ObservableCollection<IMocaFile> _files;

        public ObservableCollection<IMocaFile> Files
        {
            get => _files;
            set
            {
                _files = value;
                NotifyOfPropertyChange(() => Files);
            }
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new RelayCommand(RefreshFiles);

        private async void RefreshFiles()
        {
            IsBusy = true;
            var files = await _schemaExplorer.GetDirectory();;
            _files = new ObservableCollection<IMocaFile>(files);
            NotifyOfPropertyChange(nameof(Files));
            //use any file to get path
            var filePathTemplate = Files.FirstOrDefault()?.PathName;
            if (!string.IsNullOrEmpty(filePathTemplate))
            {
                var pathLength = filePathTemplate.LastIndexOf('\\');
                if (pathLength > 1)
                {
                    CurrentPath = filePathTemplate?.Substring(0, pathLength);
                }
            }
            IsBusy = false;
        }

        private string _currentPath;

        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value; 
                NotifyOfPropertyChange(()=>CurrentPath);
            }
        }


        private ICommand _getFileContentsSelect;
        private readonly IEventAggregator _eventAggregator;
        public ICommand GetFileContentsCommand => _getFileContentsSelect ??= new RelayCommand<IMocaFile>(c => GetFileContents(c));

        internal void GetFileContents(IMocaFile file)
        {
            if (file == null)
            {
                return;
            }
            var fileType = typeof(MocaFile);
            var dirType = typeof(MocaDirectory);
            if (file.GetType() == dirType)
            {
                OpenDirectory((MocaDirectory)file);
            }
            else
            {
                OpenFile((MocaFile)file);
            }
        }

        internal async void OpenDirectory(MocaDirectory directory)
        {
            await directory.RefreshFiles();
            Files = new ObservableCollection<IMocaFile>(directory.Files);
            NotifyOfPropertyChange(nameof(Files));
            CurrentPath = directory.PathName;
        }

        internal async void OpenFile(MocaFile file)
        {
            //TODO: Move to messaging service
            var shellView = App.Current.MainWindow;
            var vm = (ShellViewModel)shellView.DataContext;
            var response = await _mocaClient.ExecuteQuery("download file where filename = @filePath",new {filePath = file.PathName});
            var content = response.MocaResults.GetDataTable().Rows[0]["DATA"].ToString();
            var text = Encoding.UTF8.GetString(Convert.FromBase64String(content));
            /*//Check if command file
            MocaCommandFile commandDef = null;
            if (file.FileName.EndsWith(".mcmd"))
            {
                using var stream = new StringReader(text);
                var serializer = new XmlSerializer(typeof(MocaCommandFile));
                commandDef = (MocaCommandFile)serializer.Deserialize(stream);
            }

            var commandText = commandDef?.LocalSyntax;*/
            vm.NewEditor(text, false, file.FileName, null, file.PathName);
        }

        public async void NewFile()
        {
            var shellView = App.Current.MainWindow;
            var vm = (ShellViewModel)shellView.DataContext;
            var fileName = await Helpers.DialogueHelper.ShowInputDialogue("NEW FILE", "Enter new file name");
            var path = Path.Combine(CurrentPath, fileName);
            Files.Add(new MocaFile
            {
                FileName = fileName,
                PathName = path,
                Type = "F"
            });
            vm.NewEditor("", false, fileName, null, path);
        }

        public async Task HandleAsync(string message, CancellationToken cancellationToken)
        {
            switch (message)
            {
                case EventMessages.LoginEvent:
                    RefreshFiles();
                    break;
            }
        }
    }
}
