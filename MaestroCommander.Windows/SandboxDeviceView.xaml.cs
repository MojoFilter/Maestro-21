using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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

namespace MaestroCommander.Windows
{
    /// <summary>
    /// Interaction logic for SandboxDeviceView.xaml
    /// </summary>
    public partial class SandboxDeviceView : UserControl, IViewFor<SandboxDeviceViewModel>
    {
        public SandboxDeviceView()
        {
            InitializeComponent();
            this.DataContextChanged += (s, e) => this.ViewModel = e.NewValue as SandboxDeviceViewModel;
            this.WhenActivated((CompositeDisposable disposables) =>
            {

            });
        }



        public SandboxDeviceViewModel ViewModel
        {
            get { return (SandboxDeviceViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }


        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(SandboxDeviceViewModel), typeof(SandboxDeviceView), new PropertyMetadata(null));

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = value as SandboxDeviceViewModel; }

    }
}
