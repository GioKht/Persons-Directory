﻿namespace Persons.Directory.Application.Constants;

public static class ValidationMessages
{
    public const string FirstNameRequired = "FirstNameRequired";
    public const string FirstNameInvalidLength = "FirstNameInvalidLength";
    public const string FirstNameInvalidAlphabets = "FirstNameInvalidAlphabets";
    public const string LastNameRequired = "LastNameRequired";
    public const string LastNameInvalidLength = "LastNameInvalidLength";
    public const string LastNameInvalidAlphabets = "LastNameInvalidAlphabets";
    public const string GenderInvalidValue = "GenderInvalidValue";
    public const string PersonalIdMustContainExactly11NumericCharacters = "PersonalIdMustContainExactly11NumericCharacters";
    public const string PersonMustBeAtLeast18YearsOldToRegister = "PersonMustBeAtLeast18YearsOldToRegister";
    public const string PhoneNumbersCannotBeNull = "PhoneNumbersCannotBeNull";
    public const string AtLeastOnePhoneNumberMustBeProvided = "AtLeastOnePhoneNumberMustBeProvided";
    public const string NumberInvalidLength = "NumberInvalidLength";
    public const string NumberInvalidType = "NumberInvalidType";
    public const string FileSizeIsTooLarge = "FileSizeIsTooLarge";
    public const string InvalidFileType = "InvalidFileType";
    public const string NoFileIsSelected = "NoFileIsSelected";
    public const string PersonNotFoundById = "PersonNotFoundById";
    public const string PersonWithPersonalIdAlreadyExists = "PersonWithPersonalIdAlreadyExists";
    public const string RelatedPersonNotFoundById = "RelatedPersonNotFoundById";
    public const string DeleteRelatedPersonFailed = "DeleteRelatedPersonFailed";
}

