using FCG_Users.Application.Users.Requests;
using Fgc.Domain.Usuario.ObjetosDeValor;
using FluentValidation;

namespace FCG_Users.Application.Users.Validators
{
    public class AccountRequestValidator : AbstractValidator<AccountRequest>
    {
        public AccountRequestValidator()
        {
            RuleFor(x => x)
                .NotNull().WithMessage("A requisição não pode ser nula.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(150).WithMessage("O nome não pode exceder 150 caracteres.")
                .MinimumLength(2).WithMessage("O nome deve ter pelo menos 3 caracteres.");
            
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O email é obrigatório.")
                .EmailAddress().WithMessage("O email deve ser um endereço de email válido.")
                .MaximumLength(160).WithMessage("O email não pode exceder 160 caracteres.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MinimumLength(Password.MinLength).WithMessage($"A senha deve ter pelo menos {Password.MinLength} caracteres.")
                .MaximumLength(Password.MaxLength).WithMessage($"A senha não pode exceder {Password.MaxLength} caracteres.");

        }
    }
}
