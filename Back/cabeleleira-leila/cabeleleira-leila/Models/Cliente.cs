using cabeleleira_leila.Enums;

namespace cabeleleira_leila.Models;

public class Cliente : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Status Status { get; set; }
    public ICollection<Scheduling> Schedulings { get; set; } = [];

    protected Cliente()
    {
    }

    public Cliente(string name, string number, string email, string passwordHash)
    {
        Name = name;
        Number = number;
        Email = email;
        PasswordHash = passwordHash;
        Status = Status.Ativo;
    }

    public void Update(string name, string number, string email, Status status)
    {
        Name = name;
        Number = number;
        Email = email;
        Status = status;
    }
}
