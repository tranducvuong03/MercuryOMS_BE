namespace MercuryOMS.Application.Commons
{
    public class Result
    {
        public bool IsSuccess { get; init; }
        public string? Message { get; init; }

        public static Result Success(string? message = null) =>
            new Result
            {
                IsSuccess = true,
                Message = message
            };

        public static Result Failure(string message) =>
            new Result
            {
                IsSuccess = false,
                Message = message
            };
    }

    public class Result<T> : Result
    {
        public T? Value { get; init; }

        public static Result<T> Success(T value, string? message = null) =>
            new Result<T>
            {
                IsSuccess = true,
                Value = value,
                Message = message
            };

        public new static Result<T> Failure(string message) =>
            new Result<T>
            {
                IsSuccess = false,
                Message = message
            };
    }
}