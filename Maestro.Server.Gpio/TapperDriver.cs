using Maestro.Devices.Components;
using System;
using System.Device.Gpio;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Maestro.Server.Gpio
{

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
                .Do(_ => this.SetDirection(MotorDirection.Reverse))
                .Throttle(_tapExtent * 1.25)
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

    internal class MultiTapper : ITapper
    {

        public MultiTapper(params ITapper[] tappers)
        {
            _tappers = tappers;
        }

        public void Init()
        {
            foreach (var tapper in _tappers)
            {
                tapper.Init();
            }
        }

        public void Tap()
        {
            lock (_monitor)
            {
                _tappers[_index].Tap();
                _index = (_index + 1) % _tappers.Length;
            }
        }

        private int _index;
        private readonly ITapper[] _tappers;
        private readonly object _monitor = new object();
    }
}
