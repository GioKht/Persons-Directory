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

namespace Persons.Directory.Application.PersonManagement.Commands;

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repository;

    public CreatePersonCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.GetRepository<Person>();
    }

    public async Task<Unit> Handle(CreatePersonRequest request, CancellationToken cancellationToken)
    {
        var existingPerson = await _repository.FirstOrDefaultAsync(x => x.PersonalId == request.PersonalId);
        if (existingPerson is not null)
        {
            throw new HttpException($"Person with PersonalId: {request.PersonalId} already exists.", HttpStatusCode.AlreadyReported);
        }

        Person person = new(request);

        await _repository.InsertAsync(person);
        await _unitOfWork.CommitAsync();

        return new Unit();
    }
}

public class CreatePersonRequest : ICommand<Unit>
{
    public CreatePersonRequest()
    {

    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string PersonalId { get; set; }

    public DateTime BirthDate { get; set; }

    public int CityId { get; set; }

    public Gender Gender { get; set; }

    public IEnumerable<PhoneNumberModel> PhoneNumbers { get; set; }
}

public class CreatePersonRequestValidation : AbstractValidator<CreatePersonRequest>
{
    private readonly string LatinOrGeorgianAlphabetsRegex = @"^((?=[\p{IsBasicLatin}\s]*$|[\p{IsGeorgian}\s]*$)[\p{L}\s]*)$";

    private readonly IResourceManagerService _resourceManagerService;

    public CreatePersonRequestValidation(IResourceManagerService resourceManagerService)
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

        RuleFor(x => x.Gender)
            .Must(x => Enum.TryParse<Gender>(x.ToString(), out _))
            .WithMessage(GetResourceString(ValidationMessages.GenderInvalidValue));

        RuleFor(x => x.PersonalId)
            .Matches(@"^\d{11}$")
            .WithMessage(GetResourceString(ValidationMessages.PersonalIdMustContainExactly11NumericCharacters));

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Now.AddYears(-18))
            .WithMessage(GetResourceString(ValidationMessages.PersonMustBeAtLeast18YearsOldToRegister));

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