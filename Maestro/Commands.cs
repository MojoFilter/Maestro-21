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
        ResetGrip,

        // primarily response messages
        UpdateState,
        UpdateFade,
        Tap
    }
}
