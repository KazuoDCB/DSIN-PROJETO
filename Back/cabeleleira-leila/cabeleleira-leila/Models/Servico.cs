using cabeleleira_leila.Enums;

namespace cabeleleira_leila.Models
{
    public class Servico : BaseModel

    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; } // Duração em minutos
        public Status Status { get; set; }

    }
}
