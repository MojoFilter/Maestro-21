using Jacobi.Vst.Plugin.Framework;
using Maestro.Client;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Maestro.Plugin
{

    public class MaestroMap : ReactiveObject, IMaestroMap
    {
        public MaestroMap(
            INoteSource noteSource, 
            IVstPluginEvents pluginEvents,
            IMaestroClientFactory clientFactory)
        {            
            (_noteSource, _clientFactory) = (noteSource, clientFactory);

            IEnumerable<byte> range(int r) => Enumerable.Range(0, r).Select(x => (byte)x).ToList();
            this.Channels = range(15);
            this.Notes = range(127);
            pluginEvents.Opened += PluginEvents_Opened;

            _allNotes =
                this.WhenAnyValue(x => x.Channel)
                    .Select(channel =>
                        noteSource.Notes
                                  .Scan(string.Empty,
                                        (s, n) => $"{s} {n.note}"))
                    .Switch()
                    .ToProperty(this, nameof(AllNotes));

            noteSource.Notes.Subscribe(n => Debug.WriteLine(n));

            var whenIpIsValid =
                this.WhenAnyValue(x => x.DeviceIp, s => IPAddress.TryParse(s, out _));
            this.ConnectCommand = ReactiveCommand.CreateFromTask(this.ConnectAsync, whenIpIsValid);
        }

        private void PluginEvents_Opened(object? sender, EventArgs e)
        {
            _noteSource
                .Notes
                .Where(n => n.Channel == this.Channel && n.note == this.NoteNumber)
                .Do(_ => this.TapStatus = true)
                .SelectMany(async n =>
                {
                    if (_client?.IsConnected is true)
                    {
                        await _client!.TapAsync().ConfigureAwait(true);
                    }
                    return true;
                })
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Subscribe();// _ => this.TapStatus = false);
        }

        public IEnumerable<byte> Channels { get; }

        public byte Channel
        {
            get => _channel;
            set => this.RaiseAndSetIfChanged(ref _channel, value);
        }

        public IEnumerable<byte> Notes { get; }

        public byte NoteNumber
        {
            get => _noteNumber;
            set => this.RaiseAndSetIfChanged(ref _noteNumber, value);
        }

        public bool TapStatus
        {
            get => _tapStatus;
            set => this.RaiseAndSetIfChanged(ref _tapStatus, value);
        }

        public string? DeviceIp
        {
            get => _deviceIp;
            set => this.RaiseAndSetIfChanged(ref _deviceIp, value);
        }


        public string AllNotes => _allNotes.Value;

        public ICommand ConnectCommand { get; }

        private async Task ConnectAsync()
        {
            if (IPAddress.TryParse(this.DeviceIp, out var ip))
            {
                _client = _clientFactory.NewTcpMaestroClient(ip);
                await _client.ConnectAsync().ConfigureAwait(false);
            }
        }

        private bool _tapStatus;
        private byte _channel;
        private byte _noteNumber;
        private string? _deviceIp;
        private ObservableAsPropertyHelper<string> _allNotes;

        private IMaestroClient? _client;

        private readonly INoteSource _noteSource;
        private readonly IMaestroClientFactory _clientFactory;
    }
}
