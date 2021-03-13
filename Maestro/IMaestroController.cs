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
    }
}
