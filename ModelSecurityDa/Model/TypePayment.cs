namespace Entity.Model
{
    public class TypePayment
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;// Nuevo campo
        public virtual ICollection<PaymentUser>? PaymentUsers { get; set; }
    }
}