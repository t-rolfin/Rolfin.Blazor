namespace Rolfin.Blazor.FluentValidation;

public class FluentValidationError
{
    public FluentValidationError(string fieldName, string message)
    {
        Message = message;
        FieldName = fieldName;
    }
    public FluentValidationError(string message)
    {
        Message = message;
    }

    public string FieldName { get; set; }
    public string Message { get; set; }
}