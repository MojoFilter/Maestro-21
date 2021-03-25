using System.Collections.Generic;

namespace Maestro.Plugin
{
    public interface IMaestroMap
    {
        byte Channel { get; set; }
        byte NoteNumber { get; set; }
        bool TapStatus { get; set; }
        IEnumerable<byte> Channels { get; }
        IEnumerable<byte> Notes { get; }
        string AllNotes { get; }
    }

}
