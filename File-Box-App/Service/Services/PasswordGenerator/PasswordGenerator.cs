using System.Text;

namespace Service.Services.PasswordGenerator
{
    public class PasswordGenerator : IPasswordGenerator
    {
        private readonly Random m_random;
        private readonly int MIN_PASSWORD_LENGTH = 7;
        private readonly int MAX_PASSWORD_LENGTH = 10;
        public PasswordGenerator()
        {
            m_random = new Random();
        }

        public string Generate()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+[]{}|;:,.<>?";

            var password = new StringBuilder();

            for (int i = 0; i < m_random.Next(MIN_PASSWORD_LENGTH, MAX_PASSWORD_LENGTH); i++)
                password.Append(validChars[m_random.Next(validChars.Length)]);

            return password.ToString();
        }
    }
}
