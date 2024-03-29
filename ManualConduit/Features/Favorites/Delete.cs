﻿using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ManualConduit.Features.Articles;
using ManualConduit.Infra;
using ManualConduit.Infra.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ManualConduit.Features.Favorites
{
    public class Delete
    {
        public class Command : IRequest<ArticleEnvelope>
        {
            public Command(string slug)
            {
                Slug = slug;
            }

            public string Slug { get; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Slug).NotNull().NotEmpty();
            }
        }

        public class CommandHandler : IRequestHandler<Command, ArticleEnvelope>
        {
            private readonly ConduitContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public CommandHandler(ConduitContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<ArticleEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var article =
                    await _context.Articles.FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);

                if (article == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new {Article = Constants.NOT_FOUND});
                }

                var person =
                    await _context.Persons.FirstOrDefaultAsync(
                        x => x.Username == _currentUserAccessor.GetCurrentUsername(), cancellationToken);

                var favorite = await _context.ArticleFavorites.FirstOrDefaultAsync(
                    x => x.ArticleId == article.ArticleId && x.PersonId == person.PersonId, cancellationToken);

                if (favorite != null)
                {
                    _context.ArticleFavorites.Remove(favorite);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return new ArticleEnvelope(await _context.Articles.GetAllData()
                    .FirstOrDefaultAsync(x => x.ArticleId == article.ArticleId, cancellationToken));
            }
        }
    }
}
