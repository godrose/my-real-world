﻿using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ManualConduit.Infra;
using ManualConduit.Infra.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ManualConduit.Features.Comments
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Command(string slug, int id)
            {
                Slug = slug;
                Id = id;
            }

            public string Slug { get; }
            public int Id { get; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Slug).NotNull().NotEmpty();
                RuleFor(x => x.Id).NotNull().NotEmpty();
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly ConduitContext _context;

            public CommandHandler(ConduitContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var article = await _context.Articles
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);

                if (article == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new {Article = Constants.NOT_FOUND});
                }

                var comment = article.Comments.FirstOrDefault(x => x.CommentId == message.Id);
                if (comment == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Comment = Constants.NOT_FOUND });
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
