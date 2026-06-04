namespace ApiMiniPrj.Application.Common.Exceptions
{
    public abstract class ApplicationExceptionBase : Exception
    {
        protected ApplicationExceptionBase(string message) : base(message)
        {
        }
    }
}
