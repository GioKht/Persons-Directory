using FluentValidation;
using Persons.Directory.Application.Enums;

namespace Persons.Directory.Application.PersonManagement.Models;

public class PhoneNumberModel
{
    public string Number { get; set; }

    public PhoneNumberType NumberType { get; set; }
}