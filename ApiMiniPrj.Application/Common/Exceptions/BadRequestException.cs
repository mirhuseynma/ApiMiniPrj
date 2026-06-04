namespace ApiMiniPrj.Application.Common.Exceptions
{
    public class BadRequestException : ApplicationExceptionBase
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
