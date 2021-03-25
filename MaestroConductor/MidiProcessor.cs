using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using Maestro.Plugin;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace MaestroConductor
{
    

    internal sealed class MidiProcessor : IVstMidiProcessor, IVstPluginMidiSource, INoteSource
    {
        public MidiProcessor()
        {
            this.Notes.Subscribe(n => Debug.WriteLine($"Processor: {n}"));
        }

        public VstEventCollection Events { get; } = new VstEventCollection();

        public IObservable<NoteOn> Notes => _notes.AsObservable();

        public int ChannelCount => 16;

        public void Process(VstEventCollection events)
        {
            this.Events.AddRange(events);
            var notes = events.OfType<VstMidiEvent>()
                              .Select(e => e.Data)
                              .Where(IsNoteOnMessage)
                              .Select(ToNoteOn);
            notes.ToObservable().Concat(Observable.Never<NoteOn>()).Subscribe(_notes);
        }

        private static readonly ISubject<NoteOn> _notes = new Subject<NoteOn>();

        private static bool IsNoteOnMessage(byte[] data)
        {
            return data.Length > 2 && (data[0] & 0xF0) == 0x90 && data[2] > 0;
        }

        private static NoteOn ToNoteOn(byte[] data)
        {
            var channel = (byte)(data[0] & 0x0F);
            return new NoteOn(channel, data[1], data[2]);
        }
    }
}
