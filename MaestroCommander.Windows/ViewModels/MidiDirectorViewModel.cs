using DynamicData;
using Maestro;
using Maestro.Client;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaestroCommander.Windows.ViewModels
{
    public class MidiDirectorViewModel : ReactiveObject, IActivatableViewModel
    {
        public MidiDirectorViewModel(IMaestroClientFactory maestroClientFactory)
        {
            _maestroClientFactory = maestroClientFactory;
            this.LoadFileCommand = ReactiveCommand.CreateFromTask(this.LoadFileAsync);
            this.PlayCommand = ReactiveCommand.CreateFromTask(this.PlayAsync);
            this.StopCommand = ReactiveCommand.CreateFromTask(this.StopAsync);
            this.ResetCommand = ReactiveCommand.CreateFromTask(this.ResetAsync);

            var whenAddressIsValid = this.WhenAnyValue(x => x.ServerAddress, v => IPAddress.TryParse(v, out _));
            this.ConnectCommand = ReactiveCommand.CreateFromTask(this.ConnectAsync, whenAddressIsValid);

            this.WhenActivated(() =>
            {
                var disposables = new CompositeDisposable();
                
                var devices = OutputDevice.GetAll();
                this.OutputDevices = devices.Select(d => d.Name);
                this.SelectedOuputDevice = this.OutputDevices.FirstOrDefault();
                var disposeNow = new CompositeDisposable(devices);
                disposeNow.Dispose();

                this.WhenAnyValue(x => x.SwitchStatus)
                    .SelectMany(async on =>
                    {
                        if (_client is IMaestroClient)
                        {
                            if (on)
                            {
                                await _client.WakeAsync().ConfigureAwait(false);
                            }
                            else
                            {
                                await _client.SleepAsync().ConfigureAwait(false);
                            }
                        }
                        return Unit.Default;
                    }).Subscribe()
                    .DisposeWith(disposables);

                return disposables.AsEnumerable();
            });
        }

        public Interaction<Unit, string> RequestFile { get; } = new Interaction<Unit, string>();


        public ICommand LoadFileCommand { get; }

        public ICommand PlayCommand { get; }

        public ICommand StopCommand { get; }

        public ICommand ResetCommand { get; }

        public ICommand ConnectCommand { get; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public string FileName
        {
            get => _fileName;
            set => this.RaiseAndSetIfChanged(ref _fileName, value);
        }

        public IEnumerable<string> OutputDevices
        {
            get => _outputDevices;
            set => this.RaiseAndSetIfChanged(ref _outputDevices, value);
        }

        public string SelectedOuputDevice
        {
            get => _selectedOutputDevice;
            set => this.RaiseAndSetIfChanged(ref _selectedOutputDevice, value);
        }

        public Dictionary<FourBitNumber, ChannelViewModel> Channels
        {
            get => _channels;
            set => this.RaiseAndSetIfChanged(ref _channels, value);
        }

        public string ServerAddress
        {
            get => _serverAddress;
            set => this.RaiseAndSetIfChanged(ref _serverAddress, value);
        }

        public FourBitNumber SwitchChannel
        {
            get => _switchChannel;
            set => this.RaiseAndSetIfChanged(ref _switchChannel, value);
        }

        public int SwitchNote
        {
            get => _switchNote;
            set => this.RaiseAndSetIfChanged(ref _switchNote, value);
        }

        public bool SwitchStatus
        {
            get => _switchStatus;
            set => this.RaiseAndSetIfChanged(ref _switchStatus, value);
        }


        private async Task LoadFileAsync()
        {
            try
            {
                var filePath = await this.RequestFile.Handle(Unit.Default);
                this.FileName = Path.GetFileName(filePath);
                await Task.Yield();
                _midiFile = MidiFile.Read(filePath);
                this.Channels = _midiFile.GetChannels()
                                         .ToDictionary(x => x, x => new ChannelViewModel(x));
                _device = OutputDevice.GetByName(this.SelectedOuputDevice);
                _playback = _midiFile.GetPlayback(_device);
                _playback.EventPlayed += Playback_EventPlayed;
                _playback.Finished += _playback_Finished;
                _playback.Loop = true;
            }
            catch (TaskCanceledException) { }
        }

        private async Task PlayAsync()
        {
            await Task.Yield();
            this.BeginPlaying();
        }

        private async Task StopAsync()
        {
            await Task.Yield();
            _playback.Stop();
            _device.TurnAllNotesOff();
        }

        private async Task ResetAsync()
        {
            await Task.Yield();
            _playback.MoveToStart();
        }

        private void _playback_Finished(object sender, EventArgs e)
        {
        }

        private async void BeginPlaying()
        {
            await Task.Run(_playback.Play).ConfigureAwait(false);
        }

        private async Task ConnectAsync()
        {
            var address = IPAddress.Parse(this.ServerAddress);
            _client = _maestroClientFactory.NewTcpMaestroClient(address);
            await _client.ConnectAsync().ConfigureAwait(false);            
        }

        private void Playback_EventPlayed(object sender, MidiEventPlayedEventArgs e)
        {
            ChannelViewModel channel = null;
            switch (e.Event)
            {
                case NoteOnEvent noteOn:
                    channel = this.Channels[noteOn.Channel];
                    channel.NoteOn(noteOn);
                    if (noteOn.Channel == this.SwitchChannel && noteOn.NoteNumber == this.SwitchNote)
                    {
                        this.SwitchStatus = true;
                    }
                    break;
                case NoteOffEvent noteOff:
                    channel = this.Channels[noteOff.Channel];
                    channel.NoteOff(noteOff);
                    if (noteOff.Channel == this.SwitchChannel && noteOff.NoteNumber == this.SwitchNote)
                    {
                        this.SwitchStatus = false;
                    }
                    break;
            }
            
        }

        private string _fileName;
        private IEnumerable<string> _outputDevices;
        private string _selectedOutputDevice;
        private Dictionary<FourBitNumber, ChannelViewModel> _channels;
        private string _serverAddress = "192.168.86.68";
        private FourBitNumber _switchChannel = (FourBitNumber)0;
        private int _switchNote = 52;
        private bool _switchStatus;
        
        private MidiFile _midiFile;
        private OutputDevice _device;
        private Playback _playback;
        private IMaestroClient _client;

        private readonly IMaestroClientFactory _maestroClientFactory;
    }

    public class ChannelViewModel : ReactiveObject
    {
        public ChannelViewModel(FourBitNumber number)
        {
            this.Number = number;
            _noteSource.Connect()
                .ObserveOnDispatcher()
                .Bind(out _notes)
                .Subscribe();
        }

        public byte Number { get; }

        public ReadOnlyObservableCollection<ActiveNote> Notes => _notes;

        public void NoteOn(NoteOnEvent e)
        {
            _noteSource.AddOrUpdate(new ActiveNote(e.NoteNumber, e.GetNoteName().ToString(), e.GetNoteOctave(), e.Velocity / 127.0));
        }

        public void NoteOff(NoteOffEvent e)
        {
            _noteSource.RemoveKey(e.NoteNumber);
        }

        private ReadOnlyObservableCollection<ActiveNote> _notes;

        private readonly SourceCache<ActiveNote, int> _noteSource = new SourceCache<ActiveNote, int>(n => n.Number);
    }

    public record ActiveNote(int Number, string Name, int Octave, double Velocity);
}
