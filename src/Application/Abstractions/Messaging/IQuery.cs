using MediatR;

namespace Memo.Application.Abstractions.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>;