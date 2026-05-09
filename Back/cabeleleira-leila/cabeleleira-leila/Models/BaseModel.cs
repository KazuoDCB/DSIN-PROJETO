namespace cabeleleira_leila.Models
{
    public abstract class BaseModel
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public void MarkAsCreated(DateTime createdAt)
        {
            CreatedAt = createdAt;
            UpdatedAt = createdAt;
        }

        public void MarkAsUpdated(DateTime updatedAt)
        {
            UpdatedAt = updatedAt;
        }
    }
}
