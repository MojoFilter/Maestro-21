using System;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro.Server.Gpio
{
    internal class GpioMaestroController : IMaestroController
    {
        public GpioMaestroController()
        {
            _controller = new GpioController();
            _fade = PwmChannel.Create(0, 1, 400, 0.5);
            _tapper = new TapperDriver(
                TimeSpan.FromMilliseconds(100),
                _controller,
                TapperEnChannel,
                TapperIn1Pin,
                TapperIn2Pin);
        }


        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            _controller.OpenPin(LedPinNumber, PinMode.Output);
            _fade.Start();
            _tapper.Init();
        }

        public bool IsAwake() => _ledStatus;

        public void Sleep() => SetLed(false);

        public void Wake() => SetLed(true);

        public void Tap()
        {
            _tapper.Tap();
        }

        public void SetFade(double percent)
        {
            _fade.DutyCycle = percent;
            //_fadePin.SetActiveDutyCyclePercentage(percent);
        }

        public double GetFade()
        {
            //return _fadePin.GetActiveDutyCyclePercentage();
            return _fade.DutyCycle;
        }

        private void SetLed(bool isOn)
        {
            //_ledPin.Write(isOn ? GpioPinValue.High : GpioPinValue.Low);
            _controller.Write(LedPinNumber, isOn ? PinValue.High : PinValue.Low);
            _ledStatus = isOn;
        }

        //private GpioPin _ledPin;
        private bool _ledStatus = false;
        //private PwmPin _fadePin;
        private readonly GpioController _controller;
        private readonly PwmChannel _fade;

        private readonly ITapper _tapper;

        private const int LedPinNumber = 26;
        private const int FadePinNumber = 13;
        private const int TapperEnChannel = 1;
        private const int TapperIn1Pin = 5;
        private const int TapperIn2Pin = 6;

    }
}
