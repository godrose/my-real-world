using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ManualConduit.Infra;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ManualConduit.Features.Tags
{
    public class List
    {
        public class Query : IRequest<TagsEnvelope>
        {

        }
        
        public class QueryHandler : IRequestHandler<Query, TagsEnvelope>
        {
            private readonly ConduitContext _conduitContext;

            public QueryHandler(ConduitContext conduitContext)
            {
                _conduitContext = conduitContext;
            }

            public async Task<TagsEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var tags = await _conduitContext.Tags.OrderBy(t => t.TagId).AsNoTracking()
                    .ToListAsync(cancellationToken);
                return new TagsEnvelope {Tags = tags.Select(t => t.TagId).ToList()};
            }
        }
    }
}
