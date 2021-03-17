using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;

namespace MaestroCommander.Windows
{
    /// <summary>
    /// Interaction logic for CommanderView.xaml
    /// </summary>
    public partial class CommanderView : UserControl, IViewFor<ICommanderViewModel>
    {
        public CommanderView()
        {
            InitializeComponent();
            this.DataContextChanged += (s, e) => this.ViewModel = e.NewValue as ICommanderViewModel;
            this.WhenActivated((CompositeDisposable disposables) =>
            {

            });
        }



        public ICommanderViewModel ViewModel
        {
            get { return (ICommanderViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ICommanderViewModel), typeof(CommanderView), new PropertyMetadata(null));



        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = value as ICommanderViewModel;
        }
    }
}
