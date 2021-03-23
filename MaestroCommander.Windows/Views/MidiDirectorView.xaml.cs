using MaestroCommander.Windows.ViewModels;
using Microsoft.Win32;
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

namespace MaestroCommander.Windows.Views
{
    /// <summary>
    /// Interaction logic for MidiDirectorView.xaml
    /// </summary>
    public partial class MidiDirectorView : UserControl, IViewFor<MidiDirectorViewModel>
    {
        public MidiDirectorView()
        {
            InitializeComponent();
            this.SetBinding(ViewModelProperty,
                new Binding(nameof(this.DataContext))
                {
                    Source = this,
                    Mode = BindingMode.OneWay
                });

            this.WhenActivated(() =>
            {
                var trash = new CompositeDisposable();
                this.ViewModel.RequestFile
                    .RegisterHandler(ctx =>
                    {
                        var dlg = new OpenFileDialog();
                        dlg.Filter = "Midi files (.mid)|*.mid";
                        if (dlg.ShowDialog(Window.GetWindow(this)) is true)
                        {
                            ctx.SetOutput(dlg.FileName);
                        }
                        else
                        {
                            throw new TaskCanceledException();
                        }
                    });
                return new[] { trash };
            });
        }



        public MidiDirectorViewModel ViewModel
        {
            get { return (MidiDirectorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MidiDirectorViewModel), typeof(MidiDirectorView), new PropertyMetadata(null));


        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = value as MidiDirectorViewModel;
        }
    }
}
