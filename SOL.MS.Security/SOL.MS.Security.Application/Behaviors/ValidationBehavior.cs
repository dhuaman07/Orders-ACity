using FluentValidation;
using MediatR;
using SOL.MS.Security.Application.Common;

namespace SOL.MS.Security.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {            
            if (!_validators.Any())
                return await next();
           
            var context = new ValidationContext<TRequest>(request);
           
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();
            
            if (failures.Any())
            {
                var errors = failures
                    .Select(f => f.ErrorMessage)
                    .ToList();
                
                return CreateFailureResult(errors);
            }
         
            return await next();
        }

        private TResponse CreateFailureResult(List<string> errors)
        {
            var responseType = typeof(TResponse);
           
            if (responseType.IsGenericType)
            {                
                var dataType = responseType.GetGenericArguments()[0];
                var resultType = typeof(Result<>).MakeGenericType(dataType);
                
                var failureMethod = typeof(Result).GetMethod(
                    nameof(Result.Failure),
                    1,
                    new[] { typeof(List<string>), typeof(string) });

                if (failureMethod != null)
                {
                    var genericMethod = failureMethod.MakeGenericMethod(dataType);
                    return (TResponse)genericMethod.Invoke(null, new object[] { errors, "Datos inválidos" });
                }
              
                failureMethod = typeof(Result).GetMethod(
                    nameof(Result.Failure),
                    1,
                    new[] { typeof(List<string>) });

                if (failureMethod != null)
                {
                    var genericMethod = failureMethod.MakeGenericMethod(dataType);
                    return (TResponse)genericMethod.Invoke(null, new object[] { errors });
                }
            }
            else
            {      
                var failureMethod = typeof(Result).GetMethod(
                    nameof(Result.Failure),
                    new[] { typeof(List<string>), typeof(string) });

                if (failureMethod != null)
                {
                    return (TResponse)failureMethod.Invoke(null, new object[] { errors, "Datos inválidos" });
                }
              
                failureMethod = typeof(Result).GetMethod(
                    nameof(Result.Failure),
                    new[] { typeof(List<string>) });

                if (failureMethod != null)
                {
                    return (TResponse)failureMethod.Invoke(null, new object[] { errors });
                }
            }
            
            throw new InvalidOperationException(
                $"No se pudo crear Result.Failure para el tipo {responseType.Name}. " +
                "Verifica que exista Result.Failure(List<string>) o Result<T>.Failure<T>(List<string>).");
        }
    }
}
