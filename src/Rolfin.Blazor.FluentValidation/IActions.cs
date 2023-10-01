namespace Rolfin.Blazor.FluentValidation;

public interface IActions<TValue>  {
    IActions<TValue> NotNull();
    IActions<TValue> NotEmpty();
    IActions<TValue> NotDefault();
    IActions<TValue> NotEqual(TValue value);
    IActions<TValue> LessThen(TValue value);
    IActions<TValue> GreaterThen(TValue value);
    IActions<TValue> Between(TValue from, TValue to);
    IActions<TValue> WithFieldName(string fieldName);


    void WithMessage(string message);
}
