namespace Academix.Application.Common.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? Error { get; }
        public string? SuccessMessage { get; }
        public List<string> Errors { get; }

        protected Result(bool isSuccess, T? value, string? error = null, List<string>? errors = null, string? successMessage = null)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            SuccessMessage = successMessage;
            Errors = errors ?? new List<string>();
        }

        public static Result<T> Success(T value) => new(true, value);
        
        public static Result<T> Success(T value, string successMessage) => new(true, value, successMessage: successMessage);
        
        public static Result<T> Failure(string error) => new(false, default, error);
        
        public static Result<T> Failure(List<string> errors) => new(false, default, errors: errors);
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        public string? SuccessMessage { get; }
        public List<string> Errors { get; }

        protected Result(bool isSuccess, string? error = null, List<string>? errors = null, string? successMessage = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            SuccessMessage = successMessage;
            Errors = errors ?? new List<string>();
        }

        public static Result Success() => new(true);
        
        public static Result Success(string successMessage) => new(true, successMessage: successMessage);
        
        public static Result Failure(string error) => new(false, error);
        
        public static Result Failure(List<string> errors) => new(false, errors: errors);
    }
} 