using FluentValidation;
using Memo.Application.Services.Commands.Create.Command;

namespace Memo.Application.Services.Commands.Create.Validator;

internal sealed class CreateSellerValidator : AbstractValidator<CreateSellerCommand>
{
    public CreateSellerValidator()
    {
        RuleFor(s => s.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(50).WithMessage("Nome deve ter menos de 50 caracteres");
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Email é obrigatório")
            .Length(5, 10).WithMessage("Email deve conter entre 5 e 10 caracteres");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .Length(8, 30).WithMessage("Senha deve conter entre 8 e 30 caracteres")
            .Matches(@"[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[a-z]").WithMessage("Senha deve conter pelo menos uma letra minúscula")
            .Matches(@"[0-9]").WithMessage("Senha deve conter pelo menos um número");
    }
}