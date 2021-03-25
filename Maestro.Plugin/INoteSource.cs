using System;

namespace Maestro.Plugin
{
    public interface INoteSource
    {
        IObservable<NoteOn> Notes { get; }
    }

    public record NoteOn(byte Channel, byte note, byte velocity);
}
