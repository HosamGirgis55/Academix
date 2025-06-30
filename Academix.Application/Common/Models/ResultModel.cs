using System.Linq;

namespace Academix.Application.Common.Models
{
    public class ResultModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ResultModel<T> SuccessResult(T data, string message = "Operation completed successfully")
        {
            return new ResultModel<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = new List<string>()
            };
        }

        public static ResultModel<T> FailureResult(string message, List<string>? errors = null)
        {
            return new ResultModel<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors ?? new List<string>()
            };
        }

        public static ResultModel<T> FailureResult(string message, string error)
        {
            return new ResultModel<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = new List<string> { error }
            };
        }
    }

    public class ResultModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static ResultModel SuccessResult(string message = "Operation completed successfully")
        {
            return new ResultModel
            {
                Success = true,
                Message = message,
                Errors = new List<string>()
            };
        }

        public static ResultModel FailureResult(string message, List<string>? errors = null)
        {
            return new ResultModel
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        public static ResultModel FailureResult(string message, string error)
        {
            return new ResultModel
            {
                Success = false,
                Message = message,
                Errors = new List<string> { error }
            };
        }
    }

    // Extension method to convert Result<T> to ResultModel<T>
    public static class ResultExtensions
    {
        public static ResultModel<T> ToResultModel<T>(this Result<T> result, string? successMessage = null)
        {
            if (result.IsSuccess)
            {
                return ResultModel<T>.SuccessResult(
                    result.Value!,
                    successMessage ?? "Operation completed successfully"
                );
            }

            var errors = new List<string>();
            if (!string.IsNullOrEmpty(result.Error))
                errors.Add(result.Error);
            if (result.Errors?.Any() == true)
                errors.AddRange(result.Errors);

            return ResultModel<T>.FailureResult(
                result.Error ?? "Operation failed",
                errors
            );
        }

        public static ResultModel ToResultModel(this Result result, string? successMessage = null)
        {
            if (result.IsSuccess)
            {
                return ResultModel.SuccessResult(successMessage ?? "Operation completed successfully");
            }

            var errors = new List<string>();
            if (!string.IsNullOrEmpty(result.Error))
                errors.Add(result.Error);
            if (result.Errors?.Any() == true)
                errors.AddRange(result.Errors);

            return ResultModel.FailureResult(
                result.Error ?? "Operation failed",
                errors
            );
        }
    }
} 