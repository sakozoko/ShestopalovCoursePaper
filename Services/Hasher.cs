using System;
using System.Text;

namespace CourseWork.Services;

public class Hasher : IHasher
{
    public string Hash(string value, string salt)
    {
        var valueBytes = Encoding.UTF8.GetBytes(value);
        var saltBytes = Encoding.UTF8.GetBytes(salt);
        var saltValueBytes = new byte[valueBytes.Length + saltBytes.Length];

        Array.Copy(valueBytes, 0, saltValueBytes, 0, valueBytes.Length);
        Array.Copy(saltBytes, 0, saltValueBytes, valueBytes.Length, saltBytes.Length);

        var saltValueBytesToDouble = BitConverter.ToDouble(saltValueBytes);

        var hashedSaltValueBytes = 2 * Math.Sin(saltValueBytesToDouble);
        return hashedSaltValueBytes.ToString();
    }
}