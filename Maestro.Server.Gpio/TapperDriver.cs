using System;
using System.Device.Gpio;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Maestro.Server.Gpio
{
    public interface ITapper
    {
        void Init();
        void Tap();
    }

    internal class TapperDriver : MotorDriver, ITapper
    {
        public TapperDriver(TimeSpan tapExtent, GpioController controller, int pwmChannel, int in1, int in2) : base(controller, pwmChannel, in1, in2)
        {
            _tapExtent = tapExtent;
        }

        public override void Init()
        {
            base.Init();
            this.SetDirection(MotorDirection.Stop);
            this.SetSpeed(1.0);

            _tapSubject
                .Do(_ => this.SetDirection(MotorDirection.Forward))
                .Throttle(_tapExtent)
                .Do(_ => this.SetDirection(MotorDirection.Reverse))
                .Throttle(_tapExtent)
                .Do(_ => this.SetDirection(MotorDirection.Stop))
                .Subscribe();
                     
        }

        public void Tap()
        {
            _tapSubject.OnNext(Unit.Default);
        }

        private readonly ISubject<Unit> _tapSubject = new Subject<Unit>();
        private readonly TimeSpan _tapExtent;
    }
}
