namespace Service
{
    public class Util
    {
        public static readonly string DIRECTORY_BASE = @"C:\Users\hp\Desktop\file_box\";
        public static readonly long MAX_BYTE_UPLOAD_SINGLE_FILE = 1_073_741_824;
        public static readonly long MAX_BYTE_UPLOAD_MULTIPLE_FILE = 2_000_000_000;


        public static long ByteToMB(long bytes) => bytes >> 20;
        public static long ByteToGB(long bytes) => bytes >> 30;
    }
}
