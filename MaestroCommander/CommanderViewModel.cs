using DynamicData;
using DynamicData.Binding;
using Maestro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace MaestroCommander
{
    public interface ICommanderViewModel
    {
        ICommand PingCommand { get; }
        IEnumerable<IDeviceViewModel> Devices { get; }
    }



    public class CommanderViewModel : ReactiveObject, IActivatableViewModel, ICommanderViewModel
    {
        public CommanderViewModel(
            IMaestroDiscoveryClient discoveryClient,
            IDeviceViewModelFactory deviceViewModelFactory)
        {
            this.Activator = new ViewModelActivator();

            var ping = ReactiveCommand.Create(() => discoveryClient.DiscoverAsync());
            ping.ThrownExceptions.Fail();
            this.PingCommand = ping;

            var sourceCache = new SourceCache<MaestroDeviceInfo, string>(m => m.Name);

            _devices = ping
                .Switch()
                .ToObservableChangeSet(d => d.Name)
                .Transform(deviceViewModelFactory.NewViewModel)
                .ToCollection()
                .StartWithEmpty()
                .ToProperty(this, nameof(Devices));
            _devices.ThrownExceptions.Fail();

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                this.PingCommand.Execute(default);
            });
        }

        public ICommand PingCommand { get; }

        public IEnumerable<IDeviceViewModel> Devices => _devices.Value;

        public ViewModelActivator Activator { get; }

        private readonly ObservableAsPropertyHelper<IReadOnlyCollection<IDeviceViewModel>> _devices;
    }
}
