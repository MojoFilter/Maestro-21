using DynamicData;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.MusicTheory;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaestroCommander.Windows.ViewModels
{
    public class MidiDirectorViewModel : ReactiveObject, IActivatableViewModel
    {
        public MidiDirectorViewModel()
        {
            this.LoadFileCommand = ReactiveCommand.CreateFromTask(this.LoadFileAsync);
            this.PlayCommand = ReactiveCommand.CreateFromTask(this.PlayAsync);
            this.StopCommand = ReactiveCommand.CreateFromTask(this.StopAsync);
            this.WhenActivated(() =>
            {
                var disposables = new List<IDisposable>();
                var devices = OutputDevice.GetAll();
                this.OutputDevices = devices.Select(d => d.Name);
                var disposeNow = new CompositeDisposable(devices);
                disposeNow.Dispose();
                return disposables;
            });
        }

        public Interaction<Unit, string> RequestFile { get; } = new Interaction<Unit, string>();

        public ICommand LoadFileCommand { get; }

        public ICommand PlayCommand { get; }

        public ICommand StopCommand { get; }

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

        private void _playback_Finished(object sender, EventArgs e)
        {
        }

        private async void BeginPlaying()
        {
            await Task.Run(_playback.Play).ConfigureAwait(false);
        }

        private void Playback_EventPlayed(object sender, MidiEventPlayedEventArgs e)
        {
            ChannelViewModel channel = null;
            switch (e.Event)
            {
                case NoteOnEvent noteOn:
                    channel = this.Channels[noteOn.Channel];
                    channel.NoteOn(noteOn);
                    break;
                case NoteOffEvent noteOff:
                    channel = this.Channels[noteOff.Channel];
                    channel.NoteOff(noteOff);
                    break;
            }
            
        }

        private string _fileName;
        private IEnumerable<string> _outputDevices;
        private string _selectedOutputDevice;
        private Dictionary<FourBitNumber, ChannelViewModel> _channels;

        private MidiFile _midiFile;
        private OutputDevice _device;
        private Playback _playback;
    }

    public class ChannelViewModel : ReactiveObject
    {
        public ChannelViewModel(FourBitNumber number)
        {
            this.Number = number;
            _noteSource.Connect()
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
