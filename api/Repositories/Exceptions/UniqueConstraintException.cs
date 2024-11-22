namespace api.Repositories.Exceptions
{
    public class UniqueConstraintException : Exception
    {
        public static readonly int Number = 2627;

        public UniqueConstraintException(string message, Exception innerException) : base(message, innerException) { }
    }
}
