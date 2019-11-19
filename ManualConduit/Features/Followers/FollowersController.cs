using System.Threading.Tasks;
using ManualConduit.Features.Profiles;
using ManualConduit.Infra.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManualConduit.Features.Followers
{
    [Route("profiles")]
    public class FollowersController
    {
        private readonly IMediator _mediator;

        public FollowersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{username}/follow")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<ProfileEnvelope> Follow(string username)
        {
            return await _mediator.Send(new Add.Command { Username = username});
        }

        [HttpPost("{username}/unfollow")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<ProfileEnvelope> Unfollow(string username)
        {
            return await _mediator.Send(new Delete.Command {Username = username});
        }
    }
}
