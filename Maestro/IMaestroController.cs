using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro
{
    public interface IMaestroController
    {
        bool IsAwake();
        void Wake();
        void Sleep();
        Task InitAsync(CancellationToken cancellationToken = default);
        void SetFade(double percent);
        double GetFade();
        void Tap();
        Task SetGripAsync(double gripPercent);
        Task ResetGripAsync();
        void Extend();
        void FullyExtend();
        void Retract();
    }

    internal class DebugController : IMaestroController
    {
        public double GetFade() => this.Fade;

        public Task InitAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public bool IsAwake() => this.Awake;

        public void SetFade(double percent) => this.Fade = percent;

        public void Sleep() => this.Awake = false;

        public void Wake() => this.Awake = true;

        public async void Tap()
        {
            this.Wake();
            await Task.Delay(100).ConfigureAwait(false);
            this.Sleep();
        }

        public Task SetGripAsync(double gripPercent)
        {
            return Task.CompletedTask;
        }

        public Task ResetGripAsync()
        {
            throw new System.NotImplementedException();
        }

        public void Extend()
        {
            throw new System.NotImplementedException();
        }

        public void FullyExtend()
        {
            throw new System.NotImplementedException();
        }

        public void Retract()
        {
            throw new System.NotImplementedException();
        }

        public double Fade
        {
            get => _fade;
            set 
            {
                _fade = value;
                Debug.WriteLine($"Fade: {_fade}");
            }
        }

        public bool Awake
        {
            get => _awake;
            set
            {
                _awake = value;
                Debug.WriteLine($"IsAwake: {_awake}");
            }
        }

        private double _fade;
        private bool _awake;
    }
}
