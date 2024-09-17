namespace Rolfin.Blazor.FluentValidation;

internal class ValidationBuilder<TContext> : IValidationBuilder<TContext>
{
    List<FluentValidationError> _errors = new();
    TContext _context;

    public ValidationBuilder(TContext context)
    {
        _context = context;
    }


    public IActions<TValue> RuleFor<TValue>(Expression<Func<TContext, TValue>> selectedProperty)
    {
        var memberExpression = selectedProperty.Body as MemberExpression;
        var propertyInfo = memberExpression.Member as PropertyInfo;
        return new Actions<TValue>(selectedProperty.Compile().Invoke(_context), _errors, propertyInfo.Name);
    }


    internal IReadOnlyCollection<FluentValidationError> Errors => _errors.AsReadOnly();


    internal void RemoveErrorsFor(string fieldName)
    {
        var errors = _errors?.Where(x => x.FieldName.Equals(fieldName)).ToList();
        if (errors is null && errors.Any() == false) return;
        foreach (var error in errors) _errors.Remove(error);
    }
    internal void AddErrorFor(string fieldName, string message)
        => _errors.Add(new FluentValidationError(fieldName, message));
    internal void ChangeContext(TContext context) => _context = context;
}
