namespace Demographic.Domain.Exceptions
{
    public sealed class CustomValidationException : Exception
    {
        public readonly IEnumerable<ValidationError> Errors;

        public CustomValidationException(IEnumerable<ValidationError> errors) : base("One or more validation failures have occurred.")
        {
            Errors = errors;
        }

        public record ValidationError()
        {
            public string PropertyName { get; init; } = string.Empty;
            public string ErrorMessage { get; init; } = string.Empty;
        }
    }
}
