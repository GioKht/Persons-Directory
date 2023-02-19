using MediatR;
using Microsoft.AspNetCore.Mvc;
using Persons.Directory.Application.ReportManagement.Queries;

namespace Persons.Directory.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<GetRelatedPersonsResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RelatedPersons([FromQuery] GetRelatedPersonsRequest request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}
