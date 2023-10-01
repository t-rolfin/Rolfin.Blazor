namespace Rolfin.Blazor.FluentValidation;

public interface IValidationBuilder<TContext>
{
    IActions<TValue> RuleFor<TValue>(Expression<Func<TContext, TValue>> selectedProperty);
}