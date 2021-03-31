namespace Maestro.Devices.Components
{
    public interface ITapper
    {
        void Extend();
        void FullyExtend();
        void Init();
        void Retract();
        void Tap();
    }
}
