using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using JdaTools.Connection;
using JdaTools.Studio.Services;

namespace JdaTools.Studio.ViewModels
{
    public abstract class ToolsetViewBase : Screen
    {
        private MocaClient _mocaClient;
        private protected readonly SchemaExplorer _schemaExplorer;
        private readonly IEventAggregator _eventAggregator;
        private bool _isBusy;

        protected ToolsetViewBase(MocaClient mocaClient, SchemaExplorer schemaExplorer, IEventAggregator eventAggregator)
        {
            _mocaClient = mocaClient;
            _schemaExplorer = schemaExplorer;
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnPublishedThread(this);
        }
        
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
                NotifyOfPropertyChange(() => BusyComponentVisibility);
            }
        }

        public Visibility BusyComponentVisibility => IsBusy ? Visibility.Visible : Visibility.Collapsed;

        public override string ToString()
        {
            return DisplayName;
        }
    }
}