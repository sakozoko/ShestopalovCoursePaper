using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CourseWork.Services;

public class DesEncryptHandler : IEncryptHandler
{
    private readonly DES _des;
    public DesEncryptHandler()
    {
        _des= DES.Create();
        _des.KeySize=64;
        _des.GenerateKey();
        _des.GenerateIV();
    }
    public string Encrypt(string value)
    {
        var encryptor = _des.CreateEncryptor();
        var data = Encoding.UTF8.GetBytes(value);
        var encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);
        var d = Convert.ToBase64String(encrypted);
        using var file = new StreamWriter("encrypt.keys");
        var keyIv = new
        {
            Key = _des.Key.Select(c=>c.ToString()).Aggregate((a,b)=>a+b),
            IV = _des.IV.Select(c=>c.ToString()).Aggregate((a,b)=>a+b)
        };
        file.Write(keyIv);
        return d;
    }

    public string Decrypt(string value)
    {
        var decryptor = _des.CreateDecryptor();
        var data = Convert.FromBase64String(value);
        var decrypted = decryptor.TransformFinalBlock(data, 0, data.Length);
        return Encoding.UTF8.GetString(decrypted).TrimEnd('\0');;
    }
}