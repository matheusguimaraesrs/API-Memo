using MediatR;

namespace Memo.Application.Abstractions.Messaging;

public interface ICommand : IRequest, IRequest<Unit>;

public interface ICommand<out TResponse> : IRequest<TResponse>;