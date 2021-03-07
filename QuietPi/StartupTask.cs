using Maestro.Server;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace QuietPi
{
    public sealed class StartupTask : IBackgroundTask
    {

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //
            _deferral = taskInstance.GetDeferral();
            var cancellationSource = new CancellationTokenSource();
            taskInstance.Canceled += (s,e) => cancellationSource.Cancel();
            await this.Init();
            await this.Blink(cancellationSource.Token);
        }

        private async Task Init()
        {
            var gpio = await GpioController.GetDefaultAsync();
            _pin = gpio.OpenPin(PinNumber);
            _pin.SetDriveMode(GpioPinDriveMode.Output);
        }


        private async Task Blink(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _pin.Write(GpioPinValue.High);
                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                _pin.Write(GpioPinValue.Low);
                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }
        }


        private BackgroundTaskDeferral _deferral;
        private GpioPin _pin;

        private readonly TcpMaestroServer _server;
        private const int PinNumber = 7;
    }
}
