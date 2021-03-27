using System;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace Maestro.Server.Gpio
{
    public interface IGripper
    {
        Task SetGrip(double gripPercent);
        Task Reset();
        void Init();
    }

    internal class GripperDriver : MotorDriver, IGripper
    {
        public GripperDriver(GpioController controller, int pwmChannel, int in1, int in2) : base(controller, pwmChannel, in1, in2)
        {
        }

        public override async void Init()
        {
            base.Init();
            await this.Reset().ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gripPercent">
        ///     0.0: fully open
        ///     1.0: fully closed
        /// </param>
        public async Task SetGrip(double gripPercent)
        {
            var distance = gripPercent - _position;
            _position = gripPercent;
            this.SetDirection(distance > 0 ? MotorDirection.Forward : MotorDirection.Reverse);
            var travelTime = (int)(Math.Abs(distance) * FullTravelMs);
            await Task.Delay(travelTime).ConfigureAwait(false);
            this.SetDirection(MotorDirection.Stop);
        }

        public async Task Reset()
        {
            this.SetSpeed(0.75);
            this.SetDirection(MotorDirection.Reverse);
            await Task.Delay((int)(FullTravelMs * 1.25)).ConfigureAwait(false);
            this.SetDirection(MotorDirection.Forward);
            await Task.Delay(FullTravelMs).ConfigureAwait(false);
            this.SetDirection(MotorDirection.Reverse);
            await Task.Delay(FullTravelMs).ConfigureAwait(false);
            this.SetDirection(MotorDirection.Stop);
            _position = 0.0;
        }

        // 0.0 = fully open
        private double _position;

        private const int FullTravelMs = 1000;
    }
}
