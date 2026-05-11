using cabeleleira_leila.Enums;

namespace cabeleleira_leila.Models;

public class Servico : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public Status Status { get; set; }
    public ICollection<Scheduling> Schedulings { get; set; } = [];

    protected Servico()
    {
    }

    public Servico(string name, string description, decimal price, int duration, Status status)
    {
        Name = name;
        Description = description;
        Price = price;
        Duration = duration;
        Status = status;
    }

    public void Update(string name, string description, decimal price, int duration, Status status)
    {
        Name = name;
        Description = description;
        Price = price;
        Duration = duration;
        Status = status;
    }
}
