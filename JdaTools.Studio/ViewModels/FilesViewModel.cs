﻿using JdaTools.Connection;
using JdaTools.Studio.Services;
using JdaTools.Studio.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JdaTools.Studio.Models;

namespace JdaTools.Studio.ViewModels
{
    public class FilesViewModel : ViewModelBase
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
        private IEnumerable<MocaFile> _files;
        public IEnumerable<MocaFile> Files => GetFilteredFiles();

        private IEnumerable<MocaFile> GetFilteredFiles()
        {
            IEnumerable<MocaFile> files;
            if (string.IsNullOrEmpty(SearchString))
            {
                files = _files;
            }
            else if (SearchString?.FirstOrDefault() == '^')
            {
                files = _files?.Where(t => t.FileName.StartsWith(SearchString, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                files = _files?.Where(t => t.FileName.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)); //Apply filter from search
            }
            //TODO add pagination listview with 10,000 items is slow
            return files?.OrderBy(f => f.Type).ThenBy(f => f.FileName);
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new RelayCommand(RefreshFiles);

        private async void RefreshFiles()
        {
            IsBusy = true;
            _files = await _schemaExplorer.GetDirectory();
            NotifyOfPropertyChange(nameof(Files));
            IsBusy = false;
        }

        private string _searchString;

        public string SearchString
        {
            get => _searchString;
            set
            {
                SetProperty(ref _searchString, value);
                NotifyOfPropertyChange(nameof(Files));
            }
        }

        private ICommand _getFileContentsSelect;
        public ICommand GetFileContentsCommand => _getFileContentsSelect ??= new RelayCommand<MocaFile>(c => GetFileContents(c));

        internal async void GetFileContents(MocaFile file)
        {
            switch (file.Type)
            {
                case "F":
                    OpenFile(file.PathName);
                    return;
                case "D":
                    OpenDirectory(file.PathName);
                    return;
            }
        }

        internal async void OpenDirectory(string filePath)
        {
            _files = await _schemaExplorer.GetDirectory(filePath);
            NotifyOfPropertyChange(nameof(Files));
        }

        internal async void OpenFile(string filePath)
        {
            //TODO: Move to messaging service
            var shellView = App.Current.MainWindow;
            var vm = (ShellViewModel)shellView.DataContext;
            var response = await _mocaClient.ExecuteQuery("download file where filename = @filePath",new {filePath});
            var content = response.MocaResults.GetDataTable().Rows[0]["DATA"].ToString();
            var text = Encoding.UTF8.GetString(Convert.FromBase64String(content));
            vm.NewEditor(text, false, filePath);
        }

        private object selectedCommand;
        public object SelectedCommand { get => selectedCommand; set => SetProperty(ref selectedCommand, value); }
    }
}
