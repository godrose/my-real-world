﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ManualConduit.Domain;
using ManualConduit.Infra;
using ManualConduit.Infra.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ManualConduit.Features.Comments
{
    public class Create
    {
        public class Command : IRequest<CommentEnvelope>
        {
            public CommentData Comment { get; set; }

            public string Slug { get; set; }
        }

        public class CommentData
        {
            public string Body { get; set; }
        }

        public class CommentDataValidator : AbstractValidator<CommentData>
        {
            public CommentDataValidator()
            {
                RuleFor(x => x.Body).NotNull().NotEmpty();
            }
        }

        public class CommandHandler : IRequestHandler<Command, CommentEnvelope>
        {
            private readonly ConduitContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public CommandHandler(ConduitContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<CommentEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var article = await _context.Articles
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);

                if (article == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NOT_FOUND });
                }

                var author =
                    await _context.Persons.FirstAsync(x => x.Username == _currentUserAccessor.GetCurrentUsername(),
                        cancellationToken);

                var comment = new Comment
                {
                    Author = author,
                    Body = message.Comment.Body,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Comments.AddAsync(comment, cancellationToken);

                article.Comments.Add(comment);

                await _context.SaveChangesAsync(cancellationToken);

                return new CommentEnvelope(comment);
            }
        }

    }
}