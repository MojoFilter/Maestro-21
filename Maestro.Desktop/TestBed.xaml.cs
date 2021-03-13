using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Controls;

namespace Maestro.Desktop
{
    /// <summary>
    /// Interaction logic for TestBed.xaml
    /// </summary>
    public partial class TestBed : UserControl
    {
        public TestBed()
        {
            InitializeComponent();
            this.HookUpClient(App.CurrentMaestro.Client);
        }

        private async void HookUpClient(IMaestroClient client)
        {
            var subscriptions = new CompositeDisposable();

            await client.GetFadeAsync().ConfigureAwait(false);
            await client.GetStatusAsync().ConfigureAwait(false);

            IDisposable dealWith<T>(IObservable<T> src, Action<T> dealer)
                => src.Take(1).ObserveOnDispatcher().Subscribe(dealer).DisposeWith(subscriptions);

            dealWith(client.Error, ex => this.errorTextblock.Text = ex.Message);
            dealWith(client.Fade, f => this.fader.Value = f);
            dealWith(client.Status, s => this.state.IsChecked = s);

            _status.Where(s => s)
                .SelectMany(_ => client.WakeAsync().ContinueWith(_ => Unit.Default))
                .Subscribe()
                .DisposeWith(subscriptions);

            _status.Where(s => !s)
                .SelectMany(_ => client.SleepAsync().ContinueWith(_ => Unit.Default))
                .Subscribe()
                .DisposeWith(subscriptions);

            _fade.SelectMany(f => client.SetFadeAsync(f).ContinueWith(_ => Unit.Default))
                .Subscribe()
                .DisposeWith(subscriptions);


        }

        private ISubject<bool> _status = new Subject<bool>();
        private ISubject<byte> _fade = new Subject<byte>();

        private void state_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            _status.OnNext(true);
        }

        private void state_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            _status.OnNext(false);
        }

        private void fader_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            _fade.OnNext((byte)e.NewValue);
        }
    }
}
