using JdaTools.Studio.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JdaTools.Studio.Models;

namespace JdaTools.Studio.Views
{
    /// <summary>
    /// Interaction logic for FilesView.xaml
    /// </summary>
    public partial class FilesView : UserControl
    {
        public FilesView()
        {
            InitializeComponent();
        }

        private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            TextBox tBox = (TextBox)sender;
            DependencyProperty prop = TextBox.TextProperty;

            BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
            if (binding != null) { binding.UpdateSource(); }
        }

        private void FilesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var file = ((ListViewItem)sender).Content as IMocaFile;
            (DataContext as FilesViewModel).GetFileContents(file);
        }
    }
}
