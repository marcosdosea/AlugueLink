namespace Service
{
    /// <summary>
    /// Exce��o personalizada para erros de servi�o
    /// </summary>
    public class ServiceException : Exception
    {
        public ServiceException() : base()
        {
        }

        public ServiceException(string message) : base(message)
        {
        }

        public ServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}