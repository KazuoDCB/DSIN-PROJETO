using cabeleleira_leila.Enums;

namespace cabeleleira_leila.Models;

public class User : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Status Status { get; set; }
    public UserRole Role { get; set; }
    public ICollection<Scheduling> Schedulings { get; set; } = [];

    protected User()
    {
    }

    public User(string name, string number, string email, string passwordHash, UserRole role = UserRole.Cliente)
    {
        Name = name;
        Number = number;
        Email = email;
        PasswordHash = passwordHash;
        Status = Status.Ativo;
        Role = role;
    }

    public void Update(string name, string number, string email, Status status, UserRole role)
    {
        Name = name;
        Number = number;
        Email = email;
        Status = status;
        Role = role;
    }
}
