using Maestro;
using System.Reactive.Linq;
using Maestro.Client;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Reactive;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MaestroCommander.Wiindows.InputBroker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            //RxApp.MainThreadScheduler = DispatcherScheduler
            var maestroFactory = new MaestroFactory();
            var clientFactory = new MaestroClientFactory(maestroFactory.NewNetworkBusiness());
            var address = IPAddress.Parse("192.168.86.68");
            _client = clientFactory.NewTcpMaestroClient(address);
            this.InitializeComponent();
            foreach (var gamepad in Gamepad.Gamepads.TakeLast(1))
            {
                _gamepad = gamepad;
            }
            Gamepad.GamepadAdded += (s, e) => _gamepad = e;
            this.DataContext = _state;

            _state.WhenAnyValue(x => x.Fade)
                    .Where(_ => _isConnected)
                    .SelectMany(async f =>
                    {
                        await _client.SetFadeAsync(f).ConfigureAwait(false);
                        return Unit.Default;
                    }).Subscribe();

            _state.WhenAnyValue(x => x.Switch)
                    .Where(_ => _isConnected)
                    .SelectMany(async s =>
                    {
                        if (s)
                        {
                            await _client.TapAsync().ConfigureAwait(false);
                            //await _client.WakeAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            //await _client.SleepAsync().ConfigureAwait(false);
                        }
                        return Unit.Default;
                    }).Subscribe();

            foreach (var pad in Gamepad.Gamepads.Select((gamePad,index)=>(index, gamePad)))
            {
                Debug.WriteLine($"[{pad.index}] {pad.gamePad}");
            }
        }

        private async void InputLoop()
        {
            await _client.ConnectAsync().ConfigureAwait(false);
            _isConnected = true;
            while (true)
            {
                await Task.Yield();
                if (_gamepad != null)
                {
                    var reading = _gamepad.GetCurrentReading();
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        _state.Switch = reading.Buttons.HasFlag(GamepadButtons.A);
                        _state.Fade = (byte)(reading.RightTrigger * 255.0);
                    });
                }
            }
        }

        private Gamepad _gamepad;
        private ControlState _state = new ControlState();
        private IMaestroClient _client;
        private bool _isConnected;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.InputLoop();
        }
    }

    class ControlState : ReactiveObject
    {
        public byte Fade
        {
            get => _fade;
            set => this.RaiseAndSetIfChanged(ref _fade, value);
        }

        public bool Switch
        {
            get => _switch;
            set => this.RaiseAndSetIfChanged(ref _switch, value);
        }

        private byte _fade;
        private bool _switch;
    }
}
