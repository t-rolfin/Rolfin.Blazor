namespace Rolfin.Blazor.FluentValidation;

public class Actions<TValue> : IActions<TValue>
{
    TValue Value { get; set; }
    List<FluentValidationError> _errors;
    string _fieldName = string.Empty;


    public Actions(TValue val, List<FluentValidationError> errors)
    {
        Value = val;
        _errors = errors;

    }
    public Actions(TValue value, List<FluentValidationError> errors, string fieldName) : this(value, errors)
    {
        _fieldName = fieldName;
    }


    public IActions<TValue> NotNull()
    {
        if (_errors.Any(x => x.FieldName.Equals(_fieldName))) return this;
        if (Value == null) _errors.Add(new FluentValidationError(_fieldName, "Value can't be null"));
        return this;
    }
    public IActions<TValue> NotEmpty()
    {
        if (_errors.Any(x => x.FieldName.Equals(_fieldName))) return this;
        if (typeof(TValue) == typeof(string) && string.IsNullOrWhiteSpace(Value?.ToString()) == true)
        {
            _errors.Add(new FluentValidationError(_fieldName, $"Value can't be empty or whitespace."));
            return this;
        }
        else
            return NotNull();

    }
    public IActions<TValue> NotDefault()
    {
        if (_errors.Any(x => x.FieldName.Equals(_fieldName))) return this;
        if(EqualityComparer<TValue>.Default.Equals(Value, default))
            _errors.Add(new FluentValidationError(_fieldName, $"Value can't have default value."));

        return this;
    }
    public IActions<TValue> NotEqual(TValue value)
    {
        if (_errors.Any(x => x.FieldName.Equals(_fieldName))) return this;

        if (Value == null)
        {
            _errors.Add(new FluentValidationError(_fieldName, $"Value can't be equal to '{value}'"));
            return this;
        }

        if(Value?.Equals(value) == true) 
            _errors.Add(new FluentValidationError(_fieldName, $"Value can't be equal to '{value}'"));

        return this;
    }
    public IActions<TValue> Between(TValue from, TValue to)
    {
        if (_errors.Any(x => x.FieldName.Equals(_fieldName))) return this;

        Comparer<TValue> comparer = Comparer<TValue>.Default;
        
        var lessThen = comparer.Compare(Value, from);
        if (lessThen < 0){
            _errors.Add(new(_fieldName, $"Value must be greater then {from}."));
            return this;
        }

        var greaterThen = comparer.Compare(Value, to);
        if (greaterThen > 0) {
            _errors.Add(new(_fieldName, $"Value must be less then {to}."));
            return this;
        }

        return this;
    }        
    public IActions<TValue> GreaterThen(TValue value)
    {
        if (_errors.Any(x => x.FieldName.Equals(_fieldName))) return this;

        Comparer<TValue> comparer = Comparer<TValue>.Default;

        var greaterThen = comparer.Compare(Value, value);
        if (greaterThen < 0) {
            _errors.Add(new(_fieldName, $"Value must be greater then {value}."));
            return this;
        }

        return this;
    }    
    public IActions<TValue> LessThen(TValue value)
    {
        if (_errors.Any(x => x.FieldName.Equals(_fieldName))) return this;

        Comparer<TValue> comparer = Comparer<TValue>.Default;
        
        var lessThen = comparer.Compare(Value, value);
        if (lessThen > 0){
            _errors.Add(new(_fieldName, $"Value must be less then {value}."));
            return this;
        }

        return this;
    }
    public IActions<TValue> WithFieldName(string fildName)
    {
        var error = _errors.FirstOrDefault();
        if (error != null) error.FieldName = fildName;

        return this;
    }


    public void WithMessage(string message)
    {
        var error = _errors.FirstOrDefault(x => x.FieldName.Equals(_fieldName));
        if (error != null) error.Message = message;
    }
}
