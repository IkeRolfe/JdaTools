using Caliburn.Micro;
using JdaTools.Studio.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JdaTools.Studio
{
    class CaliburnBootstrapper : BootstrapperBase
    {
        public CaliburnBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}