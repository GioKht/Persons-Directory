using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Infrastructure;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.PersonManagement.Models;
using Persons.Directory.Application.Resources.GE;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Resources;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repository;

    public CreatePersonCommandHandler(IUnitOfWork unitOfWork)
        => (_unitOfWork, _repository) = (unitOfWork, unitOfWork.GetRepository<Person>());

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

    public CreatePersonRequestValidation()
    {
        var cultureName = CultureInfo.CurrentCulture.Name;

        //var errorMessage = GetErrorMessage(cultureName, "FirstNameRequired");

        RuleFor(x => x.FirstName)
           .NotNull().WithMessage(SharedResource.FirstNameRequired)
           .NotEmpty().WithMessage(SharedResource.FirstNameRequired)
           .Length(2, 50).WithMessage("First name length should be between 2 and 50 characters.")
           .Matches(LatinOrGeorgianAlphabetsRegex)
           .WithMessage("First name should not contain both English and Georgian alphabets.");

        RuleFor(x => x.LastName)
           .NotNull().WithMessage("LastName name is required.")
           .NotEmpty().WithMessage("LastName name is required.")
           .Length(2, 50).WithMessage("LastName name length should be between 2 and 50 characters.")
           .Matches(LatinOrGeorgianAlphabetsRegex)
           .WithMessage("LastName name should not contain both English and Georgian alphabets.");

        RuleFor(x => x.Gender)
            .Must(x => Enum.TryParse<Gender>(x.ToString(), out _))
            .WithMessage("Gender should be either Male or Female.");

        RuleFor(x => x.PersonalId)
            .Matches(@"^\d{11}$")
            .WithMessage("PersonalId must contain exactly 11 numeric characters.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Birth date is required.")
            .LessThan(DateTime.Now.AddYears(-18))
                .WithMessage("Person must be at least 18 years old to register.");

        RuleFor(x => x.PhoneNumbers)
            .NotNull().WithMessage("Phone numbers cannot be null")
            .Must(x => x.Any()).WithMessage("At least one phone number must be provided");

        RuleForEach(x => x.PhoneNumbers)
            .ChildRules(phoneNumber =>
            {
                phoneNumber.RuleFor(x => x.Number)
                    .Length(4, 50).WithMessage("Number length should be between 4 and 50 characters.");

                phoneNumber.RuleFor(x => x.NumberType)
                    .Must(x => Enum.TryParse<PhoneNumberType>(x.ToString(), out _))
                    .WithMessage("NumberType should be either Mobile, Office or Home.");
            });
    }

    private string GetErrorMessage(string cultureName, string key)
    {
        string resourceFileName;
        switch (cultureName)
        {
            case "ka-GE":
                resourceFileName = "Persons.Directory.Application/Resources/SharedResourceGE.resx";
                break;
            case "en-US":
            default:
                resourceFileName = "Persons.Directory.Application/Resources/SharedResourceEN.resx";
                break;
        }

        var s1 = Assembly.GetExecutingAssembly().GetManifestResourceNames();


        var s = Assembly.GetExecutingAssembly().FullName;
        var resourceManager = new ResourceManager(resourceFileName, Assembly.GetExecutingAssembly());
        return resourceManager.GetString(key);
    }
}