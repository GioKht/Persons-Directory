using System.Net;

namespace Persons.Directory.Application.Exceptions;

public class BadRequestException : Exception
{
    public HttpStatusCode Code { get; set; }

    public bool ShowMessage { get; set; }

    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, HttpStatusCode code) : base(message)
    {
        Code = code;
    }

    public BadRequestException(string message, bool showMessage = false) : base(message)
    {
        ShowMessage = showMessage;
    }
}
