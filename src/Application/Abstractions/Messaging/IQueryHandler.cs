using MediatR;

namespace Memo.Application.Abstractions.Messaging;

public interface IQueryHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse> 
    where TCommand : IQuery<TResponse>;