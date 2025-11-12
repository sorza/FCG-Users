using FCG_Users.Application.Users.Requests;
using Fgc.Domain.Usuario.ObjetosDeValor;
using FluentValidation;

namespace FCG_Users.Application.Users.Validators
{
    public class AuthRequestValidator : AbstractValidator<AuthRequest>
    {
        public AuthRequestValidator() 
        {
            RuleFor(x=> x)
                .NotNull().WithMessage("A requisição não pode ser nula.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O email é obrigatório.")
                .EmailAddress().WithMessage("O email deve ser um endereço de email válido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MinimumLength(Password.MinLength).WithMessage($"A senha deve ter pelo menos {Password.MinLength} caracteres.")
                .MaximumLength(Password.MaxLength).WithMessage($"A senha não pode exceder {Password.MaxLength} caracteres.");

        }
    }
}
