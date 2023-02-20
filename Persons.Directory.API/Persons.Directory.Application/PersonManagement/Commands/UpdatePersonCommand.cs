using FluentValidation;
using MediatR;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Infrastructure;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.PersonManagement.Models;
using System.Net;
using System.Text.Json.Serialization;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repostiory;

    public UpdatePersonCommandHandler(IUnitOfWork unitOfWork)
        => (_unitOfWork, _repostiory) = (unitOfWork, unitOfWork.GetRepository<Person>());

    public async Task<Unit> Handle(UpdatePersonRequest request, CancellationToken cancellationToken)
    {
        var person = await _repostiory.GetAsync(request.Id);

        if (person is null)
        {
            throw new BadRequestException($"Person not found by Id: {request.Id}", HttpStatusCode.NotFound);
        }

        person.SetValuesToUpdate(request);

        await _repostiory.UpdateAsync(person);
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

    public UpdatePersonRequestValidation()
    {
        RuleFor(x => x.FirstName)
           .Cascade(CascadeMode.Stop)
           .NotNull().WithMessage("First name is required.")
           .NotEmpty().WithMessage("First name is required.")
           .Length(2, 50).WithMessage("First name length should be between 2 and 50 characters.")
           .Matches(LatinOrGeorgianAlphabetsRegex)
           .WithMessage("First name should not contain both English and Georgian alphabets.");

        RuleFor(x => x.LastName)
           .Cascade(CascadeMode.Stop)
           .NotNull().WithMessage("LastName name is required.")
           .NotEmpty().WithMessage("LastName name is required.")
           .Length(2, 50).WithMessage("LastName name length should be between 2 and 50 characters.")
           .Matches(LatinOrGeorgianAlphabetsRegex)
           .WithMessage("LastName name should not contain both English and Georgian alphabets.");

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
}