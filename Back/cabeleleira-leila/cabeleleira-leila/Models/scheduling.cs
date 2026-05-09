namespace cabeleleira_leila.Models
{
    public class scheduling: BaseModel
    {
        public long ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public DateTime DataHora { get; set; }

    }
}
