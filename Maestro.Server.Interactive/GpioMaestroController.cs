using Microsoft.IoT.Lightning.Providers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices;
using Windows.Devices.Gpio;
using Windows.Devices.Pwm;

namespace Maestro.Server.Interactive
{
    internal class GpioMaestroController : IMaestroController
    {
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            if (LightningProvider.IsLightningEnabled)
            {
                LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();
            }
            //var pwm = await PwmController.GetDefaultAsync();
            var pwmControllers = await PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider());
            var pwm = pwmControllers[1];
            _fadePin = pwm.OpenPin(FadePinNumber);
            _fadePin.SetActiveDutyCyclePercentage(1.0);
            _fadePin.Start();

            var gpio = await GpioController.GetDefaultAsync();
            _ledPin = gpio.OpenPin(LedPinNumber);
            _ledPin.SetDriveMode(GpioPinDriveMode.Output);
            _ledPin.Write(GpioPinValue.Low);
        }

        public bool IsAwake() => _ledStatus;

        public void Sleep() => SetLed(false);

        public void Wake() => SetLed(true);

        public void SetFade(double percent)
        {
            _fadePin.SetActiveDutyCyclePercentage(percent);
        }

        public double GetFade()
        {
            return _fadePin.GetActiveDutyCyclePercentage();
        }

        private void SetLed(bool isOn)
        {
            _ledPin.Write(isOn ? GpioPinValue.High : GpioPinValue.Low);
            _ledStatus = isOn;
        }

        private GpioPin _ledPin;
        private bool _ledStatus = false;
        private PwmPin _fadePin;

        private const int LedPinNumber = 26;
        private const int FadePinNumber = 13;
    }
}
