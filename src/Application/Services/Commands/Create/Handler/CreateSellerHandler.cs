using Domain.Entities;
using Domain.Exceptions;
using Domain.IRepositories;
using Memo.Application.Abstractions;
using Memo.Application.Abstractions.Messaging;
using Memo.Application.Services.Commands.Create.Command;

namespace Memo.Application.Services.Commands.Create.Handler;

internal sealed class CreateSellerHandler(ISellerRepository repository, IUnitOfWork unitOfWork) :
    ICommandHandler<CreateSellerCommand, Guid>
{
    public async Task<Guid> Handle(CreateSellerCommand command, CancellationToken ct)
    {
        if (await repository.ExistsByLoginAsync(command.Login))
            throw new DomainException("Já existe um vendedor com esse usuário");
        if (await repository.ExistsByNameAsync(command.Name))
            throw new DomainException("Já existe um vendedor com esse nome");

        Seller seller = new Seller(command.Name, command.Login, command.Password);
        
        repository.Add(seller);
        await unitOfWork.SaveChangesAsync(ct);
        return seller.Id;
    }
}