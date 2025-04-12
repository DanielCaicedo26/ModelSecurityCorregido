namespace Entity.Model
{
   public class PaymentHistory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    // Verifica si esta propiedad está correctamente configurada
    public int? InformationInfractionId { get; set; }
    public virtual InformationInfraction? InformationInfraction { get; set; }
    public virtual User User { get; set; } = null!;
    }

}