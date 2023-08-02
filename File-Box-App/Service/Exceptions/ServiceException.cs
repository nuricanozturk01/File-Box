namespace Service.Exceptions
{
    public class ServiceException : Exception
    {
        private readonly string m_message;
        public ServiceException(string message) : base(message)
        {
            m_message = message;
        }

        public string GetMessage => m_message;
    }
}
