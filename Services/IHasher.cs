using System;
namespace CourseWork.Services;

public interface IHasher
{
    string Hash(string value, string salt);
}