using FluentValidation;

namespace WilderMinds.MinimalApis.FluentValidation;

/// <summary>
/// Endpoint Filter for Minimal APIs to automatically validate
/// a model type and return a validation error if it fails before
/// the Minimal API is executed.
/// </summary>
/// <typeparam name="TModel">The type to find in the API to validate with this Endpoint.</typeparam>
public class ValidationEndpointFilter<TModel> : IEndpointFilter
    where TModel : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // Find argument of Type
        var modelArgument = context.Arguments.Where(a => a.GetType() == typeof(TModel)).First() as TModel;

        // Find the validator (will throw exception, bu thtat is ok
        var validator = context.HttpContext.RequestServices.GetRequiredService<IValidator<TModel>>();

        // Test the validation
        var result = await validator.ValidateAsync(modelArgument);
        if (result.IsValid)
        {
            // Continue middleware
            return await next(context);
        }

        var validationErrors = result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(e => e.Key, 
                          e => e.Select(f => f.ErrorMessage).ToArray());

        return Results.ValidationProblem(validationErrors, $"{nameof(TModel)} failed validation");
    }
}

