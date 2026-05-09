using cabeleleira_leila.Enums;

namespace cabeleleira_leila.Models;

public class Servico : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public Status Status { get; set; }
    public ICollection<Scheduling> Schedulings { get; set; } = [];

    protected Servico()
    {
    }

    public Servico(string name, decimal price, int duration, Status status)
    {
        Name = name;
        Price = price;
        Duration = duration;
        Status = status;
    }

    public void Update(string name, decimal price, int duration, Status status)
    {
        Name = name;
        Price = price;
        Duration = duration;
        Status = status;
    }
}
