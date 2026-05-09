using System.Security.Cryptography;
using System.Text;
using cabeleleira_leila.Interfaces;
using Konscious.Security.Cryptography;

namespace cabeleleira_leila.Services;

public class Argon2IdPasswordHashService : IPasswordHashService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 3;
    private const int MemorySize = 65536;
    private const int DegreeOfParallelism = 4;

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Hash(password, salt, Iterations, MemorySize, DegreeOfParallelism);

        return $"$argon2id$v=19$m={MemorySize},t={Iterations},p={DegreeOfParallelism}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;
        if (!TryReadHash(passwordHash, out var storedHash)) return false;

        var computedHash = Hash(
            password,
            storedHash.Salt,
            storedHash.Iterations,
            storedHash.MemorySize,
            storedHash.DegreeOfParallelism);

        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash.Hash);
    }

    private static byte[] Hash(
        string password,
        byte[] salt,
        int iterations,
        int memorySize,
        int degreeOfParallelism)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            Iterations = iterations,
            MemorySize = memorySize,
            DegreeOfParallelism = degreeOfParallelism
        };

        return argon2.GetBytes(HashSize);
    }

    private static bool TryReadHash(string passwordHash, out Argon2IdHash storedHash)
    {
        storedHash = default;

        if (string.IsNullOrWhiteSpace(passwordHash)) return false;

        var parts = passwordHash.Split('$', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is not 5) return false;
        if (!string.Equals(parts[0], "argon2id", StringComparison.Ordinal)) return false;
        if (!TryReadParameters(parts[2], out var parameters)) return false;

        try
        {
            storedHash = new Argon2IdHash(
                Convert.FromBase64String(parts[3]),
                Convert.FromBase64String(parts[4]),
                parameters.Iterations,
                parameters.MemorySize,
                parameters.DegreeOfParallelism);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool TryReadParameters(string parametersValue, out Argon2IdParameters parameters)
    {
        parameters = default;
        var values = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var item in parametersValue.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var pair = item.Split('=', StringSplitOptions.RemoveEmptyEntries);

            if (pair.Length is not 2) return false;
            if (!values.TryAdd(pair[0], pair[1])) return false;
        }

        if (!TryReadInt(values, "m", out var memorySize)) return false;
        if (!TryReadInt(values, "t", out var iterations)) return false;
        if (!TryReadInt(values, "p", out var degreeOfParallelism)) return false;

        parameters = new Argon2IdParameters(iterations, memorySize, degreeOfParallelism);

        return true;
    }

    private static bool TryReadInt(IReadOnlyDictionary<string, string> values, string key, out int value)
    {
        value = default;

        if (!values.TryGetValue(key, out var stringValue)) return false;

        return int.TryParse(stringValue, out value);
    }

    private readonly record struct Argon2IdParameters(
        int Iterations,
        int MemorySize,
        int DegreeOfParallelism);

    private readonly record struct Argon2IdHash(
        byte[] Salt,
        byte[] Hash,
        int Iterations,
        int MemorySize,
        int DegreeOfParallelism);
}
