namespace Rolfin.Blazor.FluentValidation;

public partial class FluentFormValidator<T> : ComponentBase, IDisposable
{
    ValidationMessageStore _validationStore = null;


    [CascadingParameter] public EditContext Context { get; set; }
    [Parameter] public ValidationStrategy<T> Strategy { get; set; }


    public override async Task SetParametersAsync(ParameterView parameters)
    {
        EditContext previousEditContext = Context;
        await base.SetParametersAsync(parameters);

        if (Context == null)
            throw new NullReferenceException(
                $"{nameof(FluentFormValidator<T>)} must be placed within an {nameof(EditForm)}");

        if (Context != previousEditContext)
            _validationStore = new ValidationMessageStore(Context);

        if (Strategy is not null)
            InitEvents(Strategy);
    }


    public bool Validate() => Validate((T)Context.Model);
    public bool Validate(T model)
    {
        var validationResult = Strategy.Validate(model);
        var errors = Strategy.GetErrorVals();

        if (errors.Any())
        {
            _validationStore.Clear();

            foreach(var error in errors)
            {
                var field = Context.Field(error.FieldName);
                _validationStore.Add(field, error.Message);
            }
        }

        Context.NotifyValidationStateChanged();

        return validationResult;
    }
    public bool Validate(ValidationStrategy<T> strategy)
    {
        Strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        InitEvents(Strategy);
        return Validate();
    }

    public void ResetValidationFor(string fieldName)
    {
        var fieldIdentifier = Context.Field(fieldName);
        _validationStore.Clear(fieldIdentifier);
        Strategy.ClearErrorFor(fieldName);

        Context.NotifyValidationStateChanged();
    }

    void InitEvents(ValidationStrategy<T> strategy)
    {
        Context.OnValidationRequested += (s, e) => _validationStore.Clear();
        Context.OnFieldChanged += (s, e) => ValidateField(strategy, e.FieldIdentifier);
    }
    void ValidateField(ValidationStrategy<T> strategy, FieldIdentifier fieldIdentifier)
    {
        _validationStore.Clear(fieldIdentifier);
        strategy.ClearErrorFor(fieldIdentifier.FieldName);
        strategy.Validate((T)Context.Model);
        var errors = strategy.GetErrorsFor(fieldIdentifier.FieldName);
        
        if (errors.Any())
        {
            foreach (var error in errors)
                _validationStore.Add(fieldIdentifier, error.Message);
        }

        Context.NotifyValidationStateChanged();
    }


    public void Dispose()
    {
        Context.OnValidationRequested -= (s, e) => _validationStore.Clear();
        Context.OnFieldChanged -= (s, e) => ValidateField(Strategy, e.FieldIdentifier);
        Context = null;
        _validationStore = null;
        Strategy = null;
    }
}
