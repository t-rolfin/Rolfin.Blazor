namespace Rolfin.Blazor.FluentValidation;

public class ValidationStrategy<TModel>
{
    ValidationBuilder<TModel> _builder;
    Action<IValidationBuilder<TModel>> _validationBuilder;

    internal ValidationStrategy(TModel model, Action<IValidationBuilder<TModel>> validationBuilder)
    {
        _builder = new ValidationBuilder<TModel>(model);
        _validationBuilder = validationBuilder;
    }
    internal ValidationStrategy(Action<IValidationBuilder<TModel>> validationBuilder)
        : this(default, validationBuilder) { }


    public bool Validate(TModel context)
    {
        _builder.ChangeContext(context);
        return Validate();
    }
    public bool Validate()
    {
        if( _validationBuilder == default ) throw new ArgumentNullException("Pass an model as argument.");
        _validationBuilder?.Invoke(_builder);
        return !_builder.Errors.Any();
    }


    public IReadOnlyCollection<FluentValidationError> GetErrorVals() => _builder.Errors;
    public IReadOnlyCollection<FluentValidationError> GetErrorsFor(string fieldName)
        => _builder.Errors?
                .Where(x => x.FieldName.Equals(fieldName))
                .ToList();
    public void ClearErrorFor(string fieldName) => _builder.RemoveErrorsFor(fieldName);
}

public class ValidationStrategy
{
    public static ValidationStrategy<TModel> Create<TModel>(TModel model, Action<IValidationBuilder<TModel>> validationBuilder)
        => new ValidationStrategy<TModel>(model, validationBuilder);

    public static ValidationStrategy<TModel> Create<TModel>(Action<IValidationBuilder<TModel>> validationBuilder)
        => new ValidationStrategy<TModel>(validationBuilder);
}