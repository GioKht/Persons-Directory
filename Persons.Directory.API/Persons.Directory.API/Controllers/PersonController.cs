using MediatR;
using Microsoft.AspNetCore.Mvc;
using Persons.Directory.Application.PersonManagement.Queries;
using System.ComponentModel.DataAnnotations;

namespace Persons.Directory.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<GetPersonsResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Persons([FromQuery] GetPersonsRequest request)
            => Ok(await _mediator.Send(request));

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<GetPersonDetailsResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PersonDetails([Required] int id)
            => Ok(await _mediator.Send(new GetPersonDetailsRequest { Id = id }));
    }
}
