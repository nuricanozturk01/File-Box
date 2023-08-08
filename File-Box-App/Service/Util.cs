namespace Service
{
    public class Util
    {
        public static readonly string DIRECTORY_BASE = @"C:\Users\hp\WebstormProjects\filebox\src\components\file_box\";
        public static readonly long MAX_BYTE_UPLOAD_SINGLE_FILE = 1_073_741_824;
        public static readonly long MAX_BYTE_UPLOAD_MULTIPLE_FILE = 2_000_000_000;


        public static long ByteToMB(long bytes) => bytes >> 20;
        public static long ByteToGB(long bytes) => bytes >> 30;

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPassword);
        }
    }
}
