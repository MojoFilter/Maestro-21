namespace Maestro
{
    public enum Commands : byte
    {
        GetStatus,
        Wake,
        Sleep,
        SetFade,
        GetFade,

        // primarily response messages
        UpdateState,
        UpdateFade
    }
}
