
namespace FCG_Users.Application.Shared.Exceptions
{
    public sealed class ValidationException : Exception
    {
        public ValidationException(IEnumerable<ValidationError> errors)
            : base("Falha na validação dos dados.")
            => Errors = errors;

        public IEnumerable<ValidationError> Errors { get; }
    }

    public sealed record ValidationError(string Propriedade, string Erro);
}
