using Arriba.Communication;
using Arriba.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace Arriba.Observability
{
    public class ArribaApplicationObserver : IApplication
    {
        private readonly ArribaLog _logger;
        private readonly IApplication _innerService;

        public ArribaApplicationObserver(ArribaLog logger, IApplication innerService)
        {
            _logger = logger;
            _innerService = innerService;
        }

        public string Name => _innerService.Name;

        public Task<IResponse> TryProcessAsync(IRequestContext request)
        {
            return _innerService.TryProcessAsync(request);
        }
    }
}