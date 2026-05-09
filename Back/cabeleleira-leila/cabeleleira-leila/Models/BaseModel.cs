using System.Security.Cryptography.X509Certificates;

namespace cabeleleira_leila.Models
{
    public abstract class BaseModel
    {
       public long Id { get; set; }
       public DateTime CreatedAt { get; set; }
       public DateTime UpdatedAt { get; set; }

       protected void SetUpdatedAt()
       {
           UpdatedAt = DateTime.Now;
        }

    }
}
