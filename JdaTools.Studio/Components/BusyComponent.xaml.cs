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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JdaTools.Studio.Components
{
    /// <summary>
    /// Interaction logic for BusyComponent.xaml
    /// </summary>
    public partial class BusyComponent : UserControl
    {
        public BusyComponent()
        {
            InitializeComponent();
            AddAnimationToLogo();
        }

        private void AddAnimationToLogo()
        {
            DoubleAnimation da = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            BusyLogo.BeginAnimation(OpacityProperty, da);
        }
    }
}
