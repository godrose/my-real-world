using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ManualConduit.Domain;
using ManualConduit.Features.Profiles;
using ManualConduit.Infra;
using ManualConduit.Infra.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ManualConduit.Features.Followers
{
    public class Add
    {
        public class Command : IRequest<ProfileEnvelope>
        {
            public string Username { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Username).NotNull().NotEmpty();
            }
        }

        public class CommandHandler : IRequestHandler<Command, ProfileEnvelope>
        {
            private readonly ConduitContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly IProfileReader _profileReader;

            public CommandHandler(
                ConduitContext context, 
                ICurrentUserAccessor currentUserAccessor,
                IProfileReader profileReader)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
                _profileReader = profileReader;
            }

            public async Task<ProfileEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var target =
                    await _context.Persons.FirstOrDefaultAsync(x => x.Username == message.Username, cancellationToken);

                if (target == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new {User = Constants.NOT_FOUND});
                }

                var observer =
                    await _context.Persons.FirstOrDefaultAsync(
                        x => x.Username == _currentUserAccessor.GetCurrentUsername(), cancellationToken);

                if (observer == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { User = Constants.NOT_FOUND });
                }

                var followedPerson = await _context.FollowedPeople.FirstOrDefaultAsync(
                    x => x.ObserverId == observer.PersonId && x.TargetId == target.PersonId, cancellationToken);
                if (followedPerson == null)
                {
                    followedPerson = new FollowedPeople
                    {
                        Observer = observer,
                        ObserverId = observer.PersonId,
                        Target = target,
                        TargetId = target.PersonId
                    };
                    await _context.FollowedPeople.AddAsync(followedPerson, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return await _profileReader.ReadProfile(message.Username);
            }
        }
    }
}
