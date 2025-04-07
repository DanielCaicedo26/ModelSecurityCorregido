namespace Entity.Model
{
    public class PaymentUser
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Person Person { get; set; } = null!;
    }
}