using System.Windows;

namespace JdaTools.Studio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }
        public new static App Current => (App)Application.Current;
        
        private void OnStartup(object sender, StartupEventArgs e)
        {
            /*var mainWindow = Services.GetService<ShellView>();
            mainWindow.Show();*/
        }
    }
}
