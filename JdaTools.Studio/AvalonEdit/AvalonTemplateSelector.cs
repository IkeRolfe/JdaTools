using System.Windows;
using System.Windows.Controls;

namespace JdaTools.Studio.AvalonEdit
{
    public class AvalonTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ContentTemplate { get; set; }
        public DataTemplate BindingTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item is ContentPresenter ? ContentTemplate : BindingTemplate;
        }
    }
}