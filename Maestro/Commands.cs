namespace Maestro
{
    public enum Commands : byte
    {
        GetStatus,
        Wake,
        Sleep,
        SetFade,
        GetFade,
        SetGrip,

        // primarily response messages
        UpdateState,
        UpdateFade,
        Tap
    }
}
