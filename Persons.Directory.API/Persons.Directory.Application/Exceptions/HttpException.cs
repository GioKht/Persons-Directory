using System.Net;

namespace Persons.Directory.Application.Exceptions;

public class HttpException : Exception
{
    public HttpStatusCode Code { get; set; }

    public HttpException(string message, HttpStatusCode code) : base(message)
    {
        Code = code;
    }
}
