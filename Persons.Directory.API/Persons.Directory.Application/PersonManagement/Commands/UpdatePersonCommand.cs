using FluentValidation;
using MediatR;
using Persons.Directory.Application.Constants;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Infrastructure;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.PersonManagement.Models;
using Persons.Directory.Application.Services;
using System.Net;
using System.Text.Json.Serialization;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repository;
    private readonly IResourceManagerService _resourceManagerService;

    public UpdatePersonCommandHandler(IUnitOfWork unitOfWork, IResourceManagerService resourceManagerService)
        => (_unitOfWork, _repository, _resourceManagerService)
        = (unitOfWork, unitOfWork.GetRepository<Person>(), resourceManagerService);

    public async Task<Unit> Handle(UpdatePersonRequest request, CancellationToken cancellationToken)
    {
        var person = await _repository.GetAsync(request.Id);

        if (person is null)
        {
            var message = _resourceManagerService.GetString(ValidationMessages.PersonNotFoundById);
            throw new NotFoundException(string.Format(message, request.Id), true);
        }

        person.SetValuesToUpdate(request);

        await _repository.UpdateAsync(person);
        await _unitOfWork.CommitAsync();

        return new Unit();
    }
}

public class UpdatePersonRequest : ICommand<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int CityId { get; set; }

    public IEnumerable<UpdatePhoneNumberModel> PhoneNumbers { get; set; }
}

public class UpdatePersonRequestValidation : AbstractValidator<UpdatePersonRequest>
{
    private readonly string LatinOrGeorgianAlphabetsRegex = @"^((?=[\p{IsBasicLatin}\s]*$|[\p{IsGeorgian}\s]*$)[\p{L}\s]*)$";

    private readonly IResourceManagerService _resourceManagerService;

    public UpdatePersonRequestValidation(IResourceManagerService resourceManagerService)
    {
        _resourceManagerService = resourceManagerService;

        RuleFor(x => x.FirstName)
           .NotNull()
           .WithMessage(GetResourceString(ValidationMessages.FirstNameRequired))
           .NotEmpty()
           .WithMessage(GetResourceString(ValidationMessages.FirstNameRequired))
           .Length(2, 50)
           .WithMessage(GetResourceString(ValidationMessages.FirstNameInvalidLength))
           .Matches(LatinOrGeorgianAlphabetsRegex)
           .WithMessage(GetResourceString(ValidationMessages.FirstNameInvalidAlphabets));

        RuleFor(x => x.LastName)
           .NotNull()
           .WithMessage(GetResourceString(ValidationMessages.LastNameRequired))
           .NotEmpty()
           .WithMessage(GetResourceString(ValidationMessages.LastNameRequired))
           .Length(2, 50)
           .WithMessage(GetResourceString(ValidationMessages.LastNameInvalidLength))
           .Matches(LatinOrGeorgianAlphabetsRegex)
           .WithMessage(GetResourceString(ValidationMessages.LastNameInvalidAlphabets));

        RuleFor(x => x.PhoneNumbers)
            .NotNull()
            .WithMessage("Phone numbers cannot be null")
            .Must(x => x.Any())
            .WithMessage(GetResourceString(ValidationMessages.AtLeastOnePhoneNumberMustBeProvided));

        RuleForEach(x => x.PhoneNumbers)
            .ChildRules(phoneNumber =>
            {
                phoneNumber.RuleFor(x => x.Number)
                    .Length(4, 50)
                    .WithMessage(GetResourceString(ValidationMessages.NumberInvalidLength));

                phoneNumber.RuleFor(x => x.NumberType)
                    .Must(x => Enum.TryParse<PhoneNumberType>(x.ToString(), out _))
                    .WithMessage(GetResourceString(ValidationMessages.NumberInvalidType));
            });
    }

    private string GetResourceString(string key)
    {
        return _resourceManagerService.GetString(key);
    }
}