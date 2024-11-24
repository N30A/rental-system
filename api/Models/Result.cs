namespace api.Models
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        private Result(bool success, string message, T? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static Result<T> SuccessResult(T? data = default)
        {
            return SuccessResult(string.Empty, data);
        }

        public static Result<T> SuccessResult(string message, T? data = default)
        {
            return new Result<T>(true, message, data);
        }

        public static Result<T> FailureResult(string message, T? data = default)
        {
            return new Result<T>(false, message, data);
        }
    }
}
