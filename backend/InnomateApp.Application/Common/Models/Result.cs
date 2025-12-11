// Application/Common/Result.cs
using System.Collections.Generic;
using System.Linq;

namespace InnomateApp.Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string Error { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public int StatusCode { get; set; }

        public static Result<T> Success(T data) => new Result<T>
        {
            IsSuccess = true,
            Data = data,
            StatusCode = 200
        };

        public static Result<T> Failure(string error, int statusCode = 400) => new Result<T>
        {
            IsSuccess = false,
            Error = error,
            Errors = new List<string> { error },
            StatusCode = statusCode
        };

        public static Result<T> Failure(IEnumerable<string> errors, int statusCode = 400) => new Result<T>
        {
            IsSuccess = false,
            Error = string.Join("; ", errors),
            Errors = errors.ToList(),
            StatusCode = statusCode
        };

        public static Result<T> NotFound(string error = "Resource not found") => new Result<T>
        {
            IsSuccess = false,
            Error = error,
            Errors = new List<string> { error },
            StatusCode = 404
        };

        // Add this missing method
        public static Result<T> ValidationError(List<string> errors) => new Result<T>
        {
            IsSuccess = false,
            Error = "Validation failed",
            Errors = errors,
            StatusCode = 400
        };
    }
}