using Maestro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaestroCommander
{
    public class SandboxDeviceViewModel : DeviceViewModel
    {
        public SandboxDeviceViewModel(string name, IMaestroClient client) : base(name, client)
        {
            this.WhenActivated(async (CompositeDisposable disposables) =>
            {
                //client.Fade
                //      .Subscribe(f => this.Fade = f)
                //      .DisposeWith(disposables);

                //client.Status
                //      .Subscribe(s => this.Switch = s)
                //      .DisposeWith(disposables);

                this.WhenAnyValue(x => x.Fade)
                    .Where(_=>this.IsConnected)
                    .SelectMany(async f =>
                    {
                        await client.SetFadeAsync(f).ConfigureAwait(false);
                        return Unit.Default;
                    }).Subscribe()
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.Switch)
                    .Where(_ => this.IsConnected)
                    .SelectMany(async s =>
                    {
                        if (s)
                        {
                            await client.WakeAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            await client.SleepAsync().ConfigureAwait(false);
                        }
                        return Unit.Default;
                    })
                    .Subscribe()
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.IsConnected)
                    .Where(isConnected => isConnected)
                    .SelectMany(async _ =>
                    {
                        await client.GetFadeAsync().ConfigureAwait(false);
                        await client.GetStatusAsync().ConfigureAwait(false);
                        return Unit.Default;
                    })
                    .Subscribe()
                    .DisposeWith(disposables);
            });
        }

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
