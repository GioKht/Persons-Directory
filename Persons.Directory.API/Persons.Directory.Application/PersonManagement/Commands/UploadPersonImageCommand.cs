using MediatR;
using Microsoft.AspNetCore.Http;
using Persons.Directory.Application.Constants;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.Services;
using System.Net;
using System.Text.Json.Serialization;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class UploadPersonImageCommand : IRequestHandler<UploadPersonImageRequest, Unit>
{
    private readonly IRepository<Person> _repository;
    private readonly IResourceManagerService _resourceManagerService;

    public UploadPersonImageCommand(IUnitOfWork unitOfWork, IResourceManagerService resourceManagerService)
        => (_repository, _resourceManagerService)
        = (unitOfWork.GetRepository<Person>(), resourceManagerService);

    public async Task<Unit> Handle(UploadPersonImageRequest request, CancellationToken cancellationToken)
    {
        var person = await _repository.GetAsync(request.Id);

        if (person is null)
        {
            throw new BadRequestException($"Unable to upload image, person not found by Id: {request.Id}", HttpStatusCode.NotFound);
        }

        if (request.File == null || request.File.Length == 0)
        {
            var message = _resourceManagerService.GetString(ValidationMessages.NoFileIsSelected);
            throw new BadRequestException(message, HttpStatusCode.BadRequest);
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(request.File.FileName);
        if (!allowedExtensions.Contains(extension.ToLower()))
        {
            throw new ArgumentException(_resourceManagerService.GetString(ValidationMessages.InvalidFileType));
        }

        var maxFileSizeInBytes = 2097152;
        if (request.File.Length > maxFileSizeInBytes)
        {
            throw new ArgumentException(_resourceManagerService.GetString(ValidationMessages.FileSizeIsTooLarge));
        }

        var filePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"wwwroot\images");
        if (!System.IO.Directory.Exists(filePath))
        {
            System.IO.Directory.CreateDirectory(filePath);
        }

        string fileName = $"{person.FirstName}_{person.LastName}_{person.Id}.jpg";
        string path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

        // Delete existing file if it exists
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        using var stream = new FileStream(path, FileMode.Create);
        await request.File.CopyToAsync(stream);

        return new Unit();
    }
}

public class UploadPersonImageRequest : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }

    public IFormFile File { get; set; }
}
