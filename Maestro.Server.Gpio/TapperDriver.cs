using Maestro.Devices.Components;
using System;
using System.Device.Gpio;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

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
                .Do(_ => this.SetDirection(MotorDirection.Forward))
                .Throttle(_tapExtent)
                .Do(_ => this.SetDirection(MotorDirection.Reverse))
                .Throttle(_tapExtent)
                .Do(_ => this.SetDirection(MotorDirection.Stop))
                .Subscribe();

            this.Stretch();
                     
        }

        public void Tap()
        {
            _tapSubject.OnNext(Unit.Default);
        }

        public void Extend()
        {
            _extendSubject.OnNext(_tapExtent);
        }

        public void FullyExtend()
        {
            _extendSubject.OnNext(_tapExtent * 2);
        }

        public void Retract()
        {
            _retractSubject.OnNext(Unit.Default);
        }

        private async void Stretch()
        {
            this.SetDirection(MotorDirection.Reverse);
            await Task.Delay(_tapExtent * 1.5).ConfigureAwait(false);
            this.SetDirection(MotorDirection.Forward);
            await Task.Delay(_tapExtent).ConfigureAwait(false);
            this.SetDirection(MotorDirection.Reverse);
            await Task.Delay(_tapExtent).ConfigureAwait(false);
            this.SetDirection(MotorDirection.Stop);
        }

        private readonly ISubject<Unit> _tapSubject = new Subject<Unit>();
        private readonly ISubject<TimeSpan> _extendSubject = new Subject<TimeSpan>();
        private readonly ISubject<Unit> _retractSubject = new Subject<Unit>();
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

        public void Extend()
        {
            foreach(var tapper in _tappers)
            {
                tapper.Extend();
            }
        }

        public void FullyExtend()
        {
            foreach (var tapper in _tappers)
            {
                tapper.FullyExtend();
            }
        }

        public void Retract()
        {
            foreach (var tapper in _tappers)
            {
                tapper.Retract();
            }
        }

        private int _index;
        private readonly ITapper[] _tappers;
        private readonly object _monitor = new object();
    }
}
