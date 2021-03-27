using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maestro.Server.Gpio
{
    public interface IMotorDriver { }

    public enum MotorDirection
    {
        Reverse,
        Stop,
        Forward
    }

    internal class MotorDriver : IMotorDriver
    {

        public MotorDriver(GpioController controller, int pwmChannel, int in1, int in2)
        {
            (_pwmChannel, _controller, _in1, _in2) =
                (pwmChannel, controller, in1, in2);
        }

        public virtual void Init()
        {
            _pwm = PwmChannel.Create(0, _pwmChannel);
            _controller.OpenPin(_in1, PinMode.Output);
            _controller.OpenPin(_in2, PinMode.Output);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="speed">0.0 to 1.0</param>
        public void SetSpeed(double speed)
        {
            _pwm!.DutyCycle = speed;
        }

        public void SetDirection(MotorDirection direction)
        {
            var (in1, in2) = direction switch
            {
                MotorDirection.Reverse => (0, 1),
                MotorDirection.Forward => (1, 0),
                _ => (0, 0)
            };
            _controller.Write(new[] { new PinValuePair(_in1, in1), new PinValuePair(_in2, in2) });
        }

        private PwmChannel? _pwm;

        private readonly int _pwmChannel;
        private readonly GpioController _controller;
        private readonly int _in1;
        private readonly int _in2;
    }
}
