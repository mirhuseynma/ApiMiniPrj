
namespace ApiMiniPrj.Api.Test.TestDoubles;

internal sealed class StubValidator<T> : AbstractValidator<T>
{
    private readonly string[] _errors;

    public StubValidator(params string[] errors)
    {
        _errors = errors;
    }

    public async override Task<ValidationResult> ValidateAsync(ValidationContext<T> context, CancellationToken cancellation = default)
    {
        var failures = _errors.Select(error => new ValidationFailure("Test", error));
        return new ValidationResult(failures);
    }
}
