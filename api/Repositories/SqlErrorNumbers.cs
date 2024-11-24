namespace api.Repositories
{
    public enum SqlErrorNumbers
    {
        UniqueConstraintViolation = 2627,       // Violation of a UNIQUE constraint.
        ForeignKeyConstraintViolation = 547,    // Attempt to insert or update data that breaks a FOREIGN KEY relationship.
        CheckConstraintViolation = 547,         // Violation of a CHECK constraint.
        Timeout = -2,                           // The database took too long to respond or complete the operation.
        Deadlock = 1205                         // One or more transactions were terminated due to a deadlock.
    }
}
