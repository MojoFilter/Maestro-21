using System.Windows;

namespace Maestro.Plugin
{
    /// <summary>
    /// Interaction logic for MaestroConductorWindow.xaml
    /// </summary>
    public partial class MaestroConductorWindow : Window
    {
        public MaestroConductorWindow(IMaestroMap map)
        {
            InitializeComponent();
            this.DataContext = map;
        }
    }
}
