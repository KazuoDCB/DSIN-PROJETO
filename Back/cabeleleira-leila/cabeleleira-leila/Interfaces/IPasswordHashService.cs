namespace cabeleleira_leila.Interfaces;

public interface IPasswordHashService
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}
