using cabeleleira_leila.Enums;
using System.Reflection.Metadata;

namespace cabeleleira_leila.Models;

public class Cliente : BaseModel
{
    public string Name { get; set; }
    public string Number { get; set; }
    public string Email { get; set; }
    public string Passwordhash { get; set; }

    public Status Status { get; set; }
}
