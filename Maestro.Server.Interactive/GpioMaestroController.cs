using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Maestro.Server.Interactive
{
    internal class GpioMaestroController : IMaestroController
    {
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            var gpio = await GpioController.GetDefaultAsync();
            _ledPin = gpio.OpenPin(LedPinNumber);
            _ledPin.SetDriveMode(GpioPinDriveMode.Output);
            _ledPin.Write(GpioPinValue.Low);
        }

        public bool IsAwake() => _ledStatus;

        public void Sleep() => SetLed(false);

        public void Wake() => SetLed(true);

        private void SetLed(bool isOn)
        {
            _ledPin.Write(isOn ? GpioPinValue.High : GpioPinValue.Low);
            _ledStatus = isOn;
        }

        private GpioPin _ledPin;
        private bool _ledStatus = false;

        private const int LedPinNumber = 26;
    }
}
