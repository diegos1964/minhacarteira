namespace MinhaCarteira.API.Models;

public abstract class BaseEntity
{
  public Guid Id { get; protected set; }
  public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; protected set; }

  protected BaseEntity()
  {
    Id = Guid.NewGuid();
  }
}