using System.Collections.Generic;
using System.Windows.Input;

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
        string DeviceIp { get; set; }
        ICommand ConnectCommand { get; }
    }

}
