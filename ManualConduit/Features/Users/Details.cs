using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using ManualConduit.Domain;
using ManualConduit.Infra;
using ManualConduit.Infra.Errors;
using ManualConduit.Infra.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ManualConduit.Features.Users
{
    public class Details
    {
        public class Query : IRequest<UserEnvelope>
        {
            public string Username { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Username).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Query, UserEnvelope>
        {
            private readonly ConduitContext _context;
            private readonly IJwtTokenGenerator _tokenGenerator;
            private readonly IMapper _mapper;

            public Handler(
                ConduitContext context, 
                IJwtTokenGenerator tokenGenerator, 
                IMapper mapper)
            {
                _context = context;
                _tokenGenerator = tokenGenerator;
                _mapper = mapper;
            }

            public async Task<UserEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var person = await _context.Persons.AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Username == request.Username, cancellationToken);
                if (person == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new {User = Constants.NOT_FOUND});
                }

                var user = _mapper.Map<Person, User>(person);
                user.Token = await _tokenGenerator.CreateToken(person.Username);
                return new UserEnvelope(user);
            }
        }
    }
}
