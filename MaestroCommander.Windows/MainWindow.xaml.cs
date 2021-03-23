using MaestroCommander.Windows.ViewModels;
using System.Windows;

namespace MaestroCommander.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MidiDirectorViewModel vm)
        {
            InitializeComponent();
            this.Content = vm;
        }
    }
}
