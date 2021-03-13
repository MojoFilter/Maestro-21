using System.Device.Gpio;
using System.Device.Pwm;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro.Server.Gpio
{
    public class GpioMaestroController : IMaestroController
    {
        //public async Task InitAsync(CancellationToken cancellationToken = default)
        //{
        //    if (LightningProvider.IsLightningEnabled)
        //    {
        //        LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();
        //    }
        //    //var pwm = await PwmController.GetDefaultAsync();
        //    var pwmControllers = await PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider());
        //    var pwm = pwmControllers[1];
        //    _fadePin = pwm.OpenPin(FadePinNumber);
        //    _fadePin.SetActiveDutyCyclePercentage(1.0);
        //    _fadePin.Start();

        //    var gpio = await GpioController.GetDefaultAsync();
        //    _ledPin = gpio.OpenPin(LedPinNumber);
        //    _ledPin.SetDriveMode(GpioPinDriveMode.Output);
        //    _ledPin.Write(GpioPinValue.Low);
        //}

        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            _controller = new GpioController();
            _controller.OpenPin(LedPinNumber, PinMode.Output);

            _fade = PwmChannel.Create(0, 1, 400, 0.5);
            _fade.Start();
        }

        public bool IsAwake() => _ledStatus;

        public void Sleep() => SetLed(false);

        public void Wake() => SetLed(true);

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
        private GpioController _controller;
        private PwmChannel _fade;
        private const int LedPinNumber = 26;
        private const int FadePinNumber = 13;
    }
}
