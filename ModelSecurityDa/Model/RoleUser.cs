using Entity.Model;

public class RoleUser
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int UserId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual Role Role { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}