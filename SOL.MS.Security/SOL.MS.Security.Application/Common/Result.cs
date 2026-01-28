using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Common
{
    public class Result
    {
        [JsonPropertyOrder(1)]
        public bool IsSuccess { get; protected set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; protected set; }

        private List<string> _errors;

        [JsonPropertyOrder(3)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> Errors
        {
            get => _errors?.Any() == true ? _errors : null;
            protected set => _errors = value;
        }

        protected Result(bool isSuccess, string message, List<string> errors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            _errors = errors ?? new List<string>();
        }
    
        public static Result Success(string message = "Operación exitosa")
            => new Result(true, message, null);
 
        public static Result Failure(string error, string message = "Solicitud inválida")
            => new Result(false, message, new List<string> { error });
   
        public static Result ValidationFailure(List<string> errors, string message = "Datos inválidos")
            => new Result(false, message, errors);

        public static Result Failure(List<string> errors, string message = "Solicitud inválida")
            => new Result(false, message, errors);

        public static Result NotFound(string message = "Recurso no encontrado")
            => new Result(false, message, null);

        public static Result<T> Success<T>(T data, string message = "Operación exitosa")
            => new Result<T>(true, message, data, null);

        public static Result<T> Failure<T>(string error, string message = "Solicitud inválida")
            => new Result<T>(false, message, default, new List<string> { error });
     
        public static Result<T> ValidationFailure<T>(List<string> errors, string message = "Datos inválidos")
            => new Result<T>(false, message, default, errors);
    
        public static Result<T> Failure<T>(List<string> errors, string message = "Solicitud inválida")
            => new Result<T>(false, message, default, errors);

        public static Result<T> NotFound<T>(string message = "Recurso no encontrado")
            => new Result<T>(false, message, default, null);
    }

    public class Result<T> : Result
    {
        private T _data;

        [JsonPropertyOrder(4)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Data
        {
            get => IsSuccess ? _data : default;
            protected set => _data = value;
        }

        internal Result(bool isSuccess, string message, T data, List<string> errors = null)
            : base(isSuccess, message, errors)
        {
            _data = data;
        }
    }
}
