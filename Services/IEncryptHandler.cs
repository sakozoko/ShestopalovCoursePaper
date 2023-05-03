namespace CourseWork.Services;

public interface IEncryptHandler
{
    string Encrypt(string value);
    string Decrypt(string value);
}