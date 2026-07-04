using Memo.Application.Abstractions.Messaging;

namespace Memo.Application.Services.Commands.Create.Command;

public record CreateSellerCommand(string Name, string Login, string Password) : ICommand<Guid>;