namespace backend.utilities
{
    internal class InvalidPlayerRequestException(string message) : Exception(message)
    { }
}
