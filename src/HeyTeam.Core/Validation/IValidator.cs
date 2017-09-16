namespace HeyTeam.Core.Validation {
    public interface IValidator<T> {
        ValidationResult<T> Validate (T t);
    }
}