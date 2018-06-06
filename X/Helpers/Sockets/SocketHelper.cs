using System;
namespace X
{
    public static class SocketHelper
    {
        public static byte[] ToByteArray(this string s)
        {
            return System.Text.Encoding.UTF8.GetBytes(s);
        }
    }
}
