using Maestro;
using ReactiveUI;
using System.Windows.Input;
using System.Diagnostics;
using System;

namespace MaestroCommander
{
    public interface IDeviceViewModel : IActivatableViewModel
    {
        string Name { get; }
        bool IsConnected { get; }
        ICommand ConnectCommand { get; }
    }

    public class DeviceViewModel : ReactiveObject, IDeviceViewModel
    {
        public DeviceViewModel(string name, IMaestroClient client)
        {
            this.Name = name;
            this.Activator = new ViewModelActivator();
            var cmd = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    await client.ConnectAsync();
                    this.IsConnected = true;
                });
            cmd.ThrownExceptions.Fail();
            this.ConnectCommand = cmd;
        }

        public string Name { get; }

        public ICommand ConnectCommand { get; }

        public bool IsConnected
        {
            get => _isConnected;
            private set => this.RaiseAndSetIfChanged(ref _isConnected, value);
        }

        public ViewModelActivator Activator { get; }

        private bool _isConnected;
    }
}
