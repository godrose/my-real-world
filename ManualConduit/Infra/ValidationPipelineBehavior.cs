using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ManualConduit.Infra
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            throw new System.NotImplementedException();
        }
    }
}
