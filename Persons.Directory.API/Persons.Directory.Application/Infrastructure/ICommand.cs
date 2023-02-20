using MediatR;

namespace Persons.Directory.Application.Infrastructure;

public interface ICommand<out TResult> : IRequest<TResult>
{
}
