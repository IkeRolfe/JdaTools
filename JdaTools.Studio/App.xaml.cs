using JdaTools.Connection;
using JdaTools.Studio.Services;
using JdaTools.Studio.ViewModels;
using JdaTools.Studio.Views;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JdaTools.Studio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        public App()
        {
            /*ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);*/
            Services = ConfigureServices();
            Ioc.Default.ConfigureServices(Services);
            this.InitializeComponent();
        }
        public new static App Current => (App)Application.Current;
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton<MocaClient>()
                .AddSingleton<ShellViewModel>()
                .AddSingleton<ShellView>()                
                .AddSingleton<SchemaExplorer>()
                .BuildServiceProvider();
            return services;

        }
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = Services.GetService<ShellView>();
            mainWindow.Show();
        }
    }
}
