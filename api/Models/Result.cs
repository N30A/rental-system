namespace api.Models
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public ErrorCodes Error { get; set; } = ErrorCodes.None;

        private Result(bool success, string message, T? data, ErrorCodes error)
        {
            Success = success;
            Message = message;
            Data = data;
            Error = error;
        }

        public static Result<T> SuccessResult(T? data = default)
        {
            return SuccessResult(string.Empty, data);
        }

        public static Result<T> SuccessResult(string message, T? data = default)
        {
            return new Result<T>(true, message, data, ErrorCodes.None);
        }

        public static Result<T> FailureResult(string message, ErrorCodes error = ErrorCodes.UnexpectedError, T? data = default)
        {
            return new Result<T>(false, message, data, error);
        } 
    }
}
