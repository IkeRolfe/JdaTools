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

namespace JdaTools.Studio.Views
{
    /// <summary>
    /// Interaction logic for QueryView.xaml
    /// </summary>
    public partial class QueryView : UserControl
    {
        public QueryView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            using var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("JdaTools.Studio.Resources.sql.xshd");
            using var reader = new System.Xml.XmlTextReader(stream);
            QueryTextBox.SyntaxHighlighting =
                ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
                ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
        }

        private void ResultsGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string header = e.Column.Header.ToString();
            // Replace all underscores with two underscores, to prevent AccessKey handling
            var type = e.PropertyType;
            header = header + $" ({type.Name})";
            e.Column.Header = header.Replace("_", "__");

        }
        //TODO: Not wo
        private void QueryTextBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var editor = (Control)sender;
            if (editor.IsEnabled)
            {
                editor.Focus();
            }            
        }
    }
}
