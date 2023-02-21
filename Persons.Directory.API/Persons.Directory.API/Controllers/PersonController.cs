using MediatR;
using Microsoft.AspNetCore.Mvc;
using Persons.Directory.Application.Infrastructure.Models;
using Persons.Directory.Application.PersonManagement.Commands;
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
        [ProducesResponseType(typeof(FailedRequestResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Details([Required] int id)
            => Ok(await _mediator.Send(new GetPersonDetailsRequest { Id = id }));

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedRequestResponse), StatusCodes.Status208AlreadyReported)]
        public async Task<IActionResult> Create([FromBody] CreatePersonRequest request)
            => Ok(await _mediator.Send(request));

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedRequestResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateRelationship([FromBody] CreatePersonRelationshipRequest request)
            => Ok(await _mediator.Send(request));

        [Route("{id}")]
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedRequestResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([Required] int id, [FromBody] UpdatePersonRequest request)
        {
            request.Id = id;

            return Ok(await _mediator.Send(request));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedRequestResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(FailedRequestResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadImage([Required] int id, [Required] IFormFile file)
            => Ok(await _mediator.Send(new UploadPersonImageRequest
            {
                Id = id,
                File = file
            }));

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedRequestResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([Required] int id)
            => Ok(await _mediator.Send(new DeletePersonRequest
            {
                Id = id
            }));

        [Route("{personId}/{relatedPersonId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedRequestResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRelatedPerson([Required] int personId, [Required] int relatedPersonId)
           => Ok(await _mediator.Send(new DeleteRelatedPersonRequest
           {
               PersonId = personId,
               RelatedPersonId = relatedPersonId
           }));
    }
}
