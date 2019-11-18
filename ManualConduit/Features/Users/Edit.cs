using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using ManualConduit.Domain;
using ManualConduit.Infra;
using ManualConduit.Infra.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ManualConduit.Features.Users
{
    public class Edit
    {
        public class UserData
        {
            public string Username { get; set; }

            public string Email { get; set; }

            public string Password { get; set; }

            public string Bio { get; set; }

            public string Image { get; set; }
        }

        public class Command : IRequest<UserEnvelope>
        {
            public UserData User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.User).NotNull();
            }
        }

        public class Handler : IRequestHandler<Command, UserEnvelope>
        {
            private readonly ConduitContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly IPasswordHasher _passwordHasher;
            private readonly IJwtTokenGenerator _tokenGenerator;
            private readonly IMapper _mapper;

            public Handler(
                ConduitContext context, 
                ICurrentUserAccessor currentUserAccessor, 
                IPasswordHasher passwordHasher,
                IJwtTokenGenerator tokenGenerator,
                IMapper mapper)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
                _passwordHasher = passwordHasher;
                _tokenGenerator = tokenGenerator;
                _mapper = mapper;
            }

            public async Task<UserEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentUsername = _currentUserAccessor.GetCurrentUsername();
                var person = await _context.Persons.Where(x => x.Username == currentUsername).FirstOrDefaultAsync(cancellationToken);

                if (person != null)
                {
                    person.Username = request.User.Username ?? person.Username;
                    person.Email = request.User.Email ?? person.Email;
                    person.Bio = request.User.Bio ?? person.Bio;
                    person.Image = request.User.Image ?? person.Image;

                    if (!string.IsNullOrWhiteSpace(request.User.Password))
                    {
                        var salt = Guid.NewGuid().ToByteArray();
                        person.Hash = _passwordHasher.Hash(request.User.Password, salt);
                        person.Salt = salt;
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                }

                return new UserEnvelope(_mapper.Map<Person, User>(person));
            }
        }
    }
}
