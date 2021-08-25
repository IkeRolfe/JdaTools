using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;

namespace JdaTools.Studio.Helpers
{
    public static class DialogueHelper
    {
        public static MetroWindow MainWindow => (MetroWindow) Application.Current.MainWindow;
        public static Task ShowChildWindowAsync(ChildWindow childWindow) =>
            MainWindow.ShowChildWindowAsync(childWindow);

        public static Task<string> ShowInputDialogue(string title, string message)
        {

            return MainWindow.ShowInputAsync(title, message);
        }
    }
}