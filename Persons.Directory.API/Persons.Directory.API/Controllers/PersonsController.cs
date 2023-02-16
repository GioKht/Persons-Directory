using MediatR;
using Microsoft.AspNetCore.Mvc;
using Persons.Directory.Application.PersonManagement.Queries;

namespace Persons.Directory.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PersonsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<GetPersonsResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Persons([FromQuery] GetPersonsRequest request)
            => Ok(await _mediator.Send(request));
    }
}
