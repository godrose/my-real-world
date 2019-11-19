using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ManualConduit.Features.Profiles
{
    [Route("profile")]
    public class ProfilesController
    {
        private readonly IMediator _mediator;

        public ProfilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{username}")]
        public async Task<ProfileEnvelope> GetProfile(string username)
        {
            return await _mediator.Send(new Details.Query {Username = username});
        }
    }
}
